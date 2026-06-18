using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using WorkLogApp.Data;
using WorkLogApp.Models;
using WorkLogApp.Services;

namespace WorkLogApp.Controllers;

[Authorize]
public class WorkLogController(AppDbContext db, OllamaService ai) : Controller
{
    private int Uid => int.Parse(User.FindFirst("UserId")!.Value);

    public async Task<IActionResult> Index()
    {
        var uid = Uid;
        var today = DateTime.UtcNow.Date;
        int dow = (int)today.DayOfWeek;
        var weekStart = today.AddDays(dow == 0 ? -6 : -(dow - 1));

        var entries = await db.WorkEntries.Include(e => e.Tasks)
            .Where(e => e.UserId == uid)
            .OrderByDescending(e => e.LogDate).ThenByDescending(e => e.CreatedAt)
            .Take(60).ToListAsync();

        ViewBag.TodayHours = entries.Where(e => e.LogDate.Date == today).Sum(e => (double)e.TotalHours);
        ViewBag.WeekHours  = entries.Where(e => e.LogDate.Date >= weekStart).Sum(e => (double)e.TotalHours);
        ViewBag.Streak     = await GetStreak(uid);
        ViewBag.Projects   = await db.WorkTasks
                                .Where(t => t.WorkEntry.UserId == uid && t.Project != null)
                                .Select(t => t.Project!)
                                .Distinct().OrderBy(p => p).Take(20).ToListAsync();
        ViewBag.Section    = "worklog";

        var notes = await db.Notes.Include(n => n.ChecklistItems)
            .Where(n => n.UserId == uid && !n.IsArchived && !n.IsDeleted)
            .OrderByDescending(n => n.IsPinned).ThenByDescending(n => n.UpdatedAt)
            .Take(50).ToListAsync();
        ViewBag.Notes = notes;

        return View(entries);
    }

    public async Task<IActionResult> Timesheet(int weekOffset = 0)
    {
        var uid = Uid;
        var today = DateTime.UtcNow.Date;
        int dow = (int)today.DayOfWeek;
        var weekStart = today.AddDays(dow == 0 ? -6 : -(dow - 1)).AddDays(weekOffset * 7);
        var weekEnd   = weekStart.AddDays(6);

        var tasks = await db.WorkTasks.Include(t => t.WorkEntry)
            .Where(t => t.WorkEntry.UserId == uid
                     && t.WorkEntry.LogDate >= weekStart
                     && t.WorkEntry.LogDate <= weekEnd)
            .ToListAsync();

        ViewBag.WeekStart  = weekStart;
        ViewBag.WeekEnd    = weekEnd;
        ViewBag.WeekOffset = weekOffset;
        ViewBag.Section    = "timesheet";
        return View(tasks);
    }

    public async Task<IActionResult> TimesheetJson(int weekOffset = 0)
    {
        var uid = Uid;
        var today = DateTime.UtcNow.Date;
        int dow = (int)today.DayOfWeek;
        var weekStart = today.AddDays(dow == 0 ? -6 : -(dow - 1)).AddDays(weekOffset * 7);
        var weekEnd   = weekStart.AddDays(6);

        var tasks = await db.WorkTasks.Include(t => t.WorkEntry)
            .Where(t => t.WorkEntry.UserId == uid
                     && t.WorkEntry.LogDate >= weekStart
                     && t.WorkEntry.LogDate <= weekEnd)
            .ToListAsync();

        var days = Enumerable.Range(0, 7).Select(i => weekStart.AddDays(i)).ToList();

        var byProject = tasks
            .GroupBy(t => t.Project ?? "(no project)")
            .OrderBy(g => g.Key)
            .Select(g => new {
                name  = g.Key,
                total = (double)g.Sum(t => t.Hours),
                hours = days.ToDictionary(
                    d => d.ToString("yyyy-MM-dd"),
                    d => (double)g.Where(t => t.WorkEntry.LogDate.Date == d.Date).Sum(t => t.Hours))
            })
            .ToList();

        var dayTotals = days.ToDictionary(
            d => d.ToString("yyyy-MM-dd"),
            d => (double)tasks.Where(t => t.WorkEntry.LogDate.Date == d.Date).Sum(t => t.Hours));

        var grandTotal = tasks.Sum(t => (double)t.Hours);

        // Build export text
        var ordered = tasks.OrderBy(t => t.WorkEntry.LogDate).ThenBy(t => t.SortOrder).ToList();
        var jiraLines  = ordered.Select(t => (t.Project != null ? "[" + t.Project + "] " : "") + t.Description + " - " + t.Hours + "h");
        var slackLines = ordered.Select(t => "• " + t.Description + " (" + t.Hours + "h)" + (t.Project != null ? " [" + t.Project + "]" : ""));

        return Json(new {
            weekStart  = weekStart.ToString("MMM d"),
            weekEnd    = weekEnd.ToString("MMM d, yyyy"),
            weekOffset = weekOffset,
            days       = days.Select(d => new {
                date  = d.ToString("yyyy-MM-dd"),
                label = d.ToString("ddd"),
                @short = d.ToString("M/d")
            }),
            byProject  = byProject,
            dayTotals  = dayTotals,
            grandTotal = grandTotal,
            jiraText   = string.Join("\n", jiraLines),
            slackText  = "*This week:*\n" + string.Join("\n", slackLines)
        });
    }

    [HttpPost]
    public async Task<IActionResult> Log([FromBody] WlLogReq req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.Text)) return BadRequest();
        var date = req.Date?.Date ?? DateTime.UtcNow.Date;
        var entry = new WorkEntry { UserId = Uid, RawText = req.Text.Trim(), LogDate = date };
        try
        {
            var sys = """
                You are a timesheet parser. Extract individual tasks from the user's work description.
                Return ONLY a JSON array (no markdown, no extra text):
                [{"description":"string","hours":1.5,"project":"string or null","category":"development|meeting|review|planning|ops|general"}]
                If hours not given, estimate reasonably. Total hours should be 1-10.
                """;
            var json = await ai.ChatAsync(sys, [], req.Text, ct);
            var s = json.IndexOf('['); var e2 = json.LastIndexOf(']');
            if (s >= 0 && e2 > s)
            {
                var arr = System.Text.Json.JsonSerializer.Deserialize<List<WlParsed>>(json[s..(e2+1)],
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (arr is { Count: > 0 })
                {
                    for (int i = 0; i < arr.Count; i++)
                        entry.Tasks.Add(new WorkTask
                        {
                            Description = arr[i].Description ?? "",
                            Hours       = arr[i].Hours,
                            Project     = arr[i].Project,
                            Category    = arr[i].Category ?? "general",
                            SortOrder   = i
                        });
                    entry.IsAiParsed = true;
                }
            }
        }
        catch { /* AI unavailable */ }

        if (!entry.Tasks.Any())
            entry.Tasks.Add(new WorkTask { Description = req.Text, Hours = 0, Category = "general" });

        entry.TotalHours = entry.Tasks.Sum(t => t.Hours);
        db.WorkEntries.Add(entry);
        await db.SaveChangesAsync(ct);
        return Json(ToDto(entry));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateTask([FromBody] WlUpdateTaskReq req)
    {
        var task = await db.WorkTasks.Include(t => t.WorkEntry)
            .FirstOrDefaultAsync(t => t.Id == req.Id && t.WorkEntry.UserId == Uid);
        if (task is null) return NotFound();
        if (req.Description is not null) task.Description = req.Description;
        if (req.Hours.HasValue)          task.Hours = req.Hours.Value;
        if (req.Project is not null)     task.Project = req.Project == "" ? null : req.Project;
        if (req.Category is not null)    task.Category = req.Category;
        task.WorkEntry.TotalHours = await db.WorkTasks
            .Where(t => t.WorkEntryId == task.WorkEntryId).SumAsync(t => t.Hours);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEntry([FromBody] WlIdReq req)
    {
        var e = await db.WorkEntries.FirstOrDefaultAsync(x => x.Id == req.Id && x.UserId == Uid);
        if (e is null) return NotFound();
        db.WorkEntries.Remove(e);
        await db.SaveChangesAsync();
        return Ok();
    }

    public async Task<IActionResult> ExportCsv()
    {
        var uid = Uid;
        var tasks = await db.WorkTasks.Include(t => t.WorkEntry)
            .Where(t => t.WorkEntry.UserId == uid)
            .OrderBy(t => t.WorkEntry.LogDate).ThenBy(t => t.SortOrder)
            .ToListAsync();
        var sb = new StringBuilder("Date,Project,Description,Hours,Category\n");
        foreach (var t in tasks)
            sb.AppendLine($"{t.WorkEntry.LogDate:yyyy-MM-dd},{Q(t.Project ?? "")},{Q(t.Description)},{t.Hours:F2},{t.Category}");
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", $"worklog-{DateTime.UtcNow:yyyy-MM-dd}.csv");
    }

    private static string Q(string s) =>
        s.Contains(',') || s.Contains('"') ? $"\"{s.Replace("\"", "\"\"")}\"" : s;

    private async Task<int> GetStreak(int uid)
    {
        var today = DateTime.UtcNow.Date;
        var dates = await db.WorkEntries
            .Where(e => e.UserId == uid && e.LogDate <= today)
            .Select(e => e.LogDate.Date).Distinct()
            .OrderByDescending(d => d).Take(365).ToListAsync();
        int streak = 0;
        var check = today;
        if (!dates.Contains(check)) check = check.AddDays(-1);
        while (dates.Contains(check)) { streak++; check = check.AddDays(-1); }
        return streak;
    }

    private static object ToDto(WorkEntry e) => new
    {
        e.Id, e.RawText, e.TotalHours, e.IsAiParsed,
        logDate   = e.LogDate.ToString("yyyy-MM-dd"),
        relDate   = e.RelativeDate(),
        createdAt = e.CreatedAt.ToString("h:mm tt"),
        tasks = e.Tasks.OrderBy(t => t.SortOrder)
                       .Select(t => new { t.Id, t.Description, t.Hours, t.Project, t.Category })
    };
}

public record WlLogReq(string Text, DateTime? Date);
public record WlUpdateTaskReq(int Id, string? Description, float? Hours, string? Project, string? Category);
public record WlIdReq(int Id);
public record WlParsed(string? Description, float Hours, string? Project, string? Category);
