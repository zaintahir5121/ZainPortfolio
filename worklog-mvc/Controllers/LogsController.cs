using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;
using WorkLogApp.Models.ViewModels;
using WorkLogApp.Services;

namespace WorkLogApp.Controllers;

[Authorize]
public class LogsController(AppDbContext db, IOllamaService ollama) : Controller
{
    private int  CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin       => User.IsInRole("admin");

    /* ── Dashboard ── */
    [HttpGet]
    public async Task<IActionResult> Index(string? period, string? q)
    {
        // Fire any recurring entries due today (no-op if already fired)
        var fired = await RecurringController.FireTodayAsync(db, CurrentUserId);
        if (fired > 0)
            TempData["Toast"] = $"↺ {fired} recurring {(fired == 1 ? "entry" : "entries")} auto-logged for today";

        var today = DateOnly.FromDateTime(DateTime.Today);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        // All of the current user's (or everyone's) logs for stats
        var allMine = db.LogEntries.Where(l => IsAdmin || l.UserId == CurrentUserId);

        var query = allMine.Include(l => l.User).AsQueryable();

        query = period switch
        {
            "today" => query.Where(l => l.Date == today),
            "week"  => query.Where(l => l.Date >= weekStart),
            "month" => query.Where(l => l.Date >= monthStart),
            _       => query,
        };

        if (!string.IsNullOrWhiteSpace(q))
        {
            var sq = q.ToLower();
            query = query.Where(l =>
                l.Project.ToLower().Contains(sq) ||
                l.Description.ToLower().Contains(sq) ||
                l.Tags.ToLower().Contains(sq));
        }

        var logs  = await query.OrderByDescending(l => l.Date).ThenByDescending(l => l.CreatedAt).ToListAsync();
        var stats = await allMine.ToListAsync();

        // ── Insights ──────────────────────────────────────────────
        var lastWeekStart = weekStart.AddDays(-7);
        var lastWeekEnd   = weekStart.AddDays(-1);
        var monthStart    = new DateOnly(today.Year, today.Month, 1);

        var weekHours     = stats.Where(l => l.Date >= weekStart && l.Date <= today).Sum(l => l.Hours);
        var lastWeekHours = stats.Where(l => l.Date >= lastWeekStart && l.Date <= lastWeekEnd).Sum(l => l.Hours);

        var topProj = stats
            .Where(l => l.Date >= monthStart)
            .GroupBy(l => l.Project)
            .Select(g => new { Name = g.Key, Hours = g.Sum(e => e.Hours) })
            .OrderByDescending(x => x.Hours)
            .FirstOrDefault();

        // Streak: consecutive logged days ending today or yesterday
        var logDates  = new HashSet<DateOnly>(stats.Select(l => l.Date));
        var checkDay  = logDates.Contains(today) ? today : today.AddDays(-1);
        var streak    = 0;
        while (logDates.Contains(checkDay)) { streak++; checkDay = checkDay.AddDays(-1); }

        // Warn if it's after 4 pm and nothing logged today
        var warnNotLogged = DateTime.Now.Hour >= 16 && stats.All(l => l.Date != today);

        // Recent distinct projects ordered by last use date
        var recentProjects = stats
            .GroupBy(l => l.Project)
            .Select(g => new { Name = g.Key, LastDate = g.Max(e => e.Date), TotalHours = g.Sum(e => e.Hours) })
            .OrderByDescending(x => x.LastDate)
            .Select(x => x.Name)
            .Take(15)
            .ToList();

        var vm = new DashboardViewModel
        {
            Logs             = logs,
            Period           = period ?? "all",
            SearchQuery      = q ?? "",
            UserName         = User.FindFirstValue(ClaimTypes.Name) ?? "",
            IsAdmin          = IsAdmin,
            TodayHours       = stats.Where(l => l.Date == today).Sum(l => l.Hours),
            WeekLogsCount    = stats.Count(l => l.Date >= weekStart),
            ProjectsCount    = stats.Select(l => l.Project).Distinct().Count(),
            Streak           = streak,
            WeekHours        = weekHours,
            LastWeekHours    = lastWeekHours,
            TopProject       = topProj?.Name ?? "",
            TopProjectHours  = topProj?.Hours ?? 0,
            WarnNotLogged    = warnNotLogged,
            RecentProjects   = recentProjects,
        };

        ViewBag.TodayHours    = vm.TodayHours;
        ViewBag.WeekLogsCount = vm.WeekLogsCount;
        ViewBag.ProjectsCount = vm.ProjectsCount;

        return View(vm);
    }

    /* ── Add ── */
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddLogViewModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in all required fields.";
            return RedirectToAction(nameof(Index));
        }

        db.LogEntries.Add(new LogEntry
        {
            UserId      = CurrentUserId,
            Date        = model.Date,
            Project     = model.Project.Trim(),
            Description = model.Description.Trim(),
            Hours       = model.Hours,
            Tags        = model.Tags.Trim(),
            CreatedAt   = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
        TempData["Toast"] = "Work logged!";
        return RedirectToAction(nameof(Index));
    }

    /* ── Delete ── */
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await db.LogEntries.FindAsync(id);
        if (entry is null) return NotFound();
        if (entry.UserId != CurrentUserId && !IsAdmin) return Forbid();

        db.LogEntries.Remove(entry);
        await db.SaveChangesAsync();

        TempData["Toast"] = "Entry deleted.";
        return RedirectToAction(nameof(Index));
    }

    /* ── AI: improve description ── */
    [HttpPost]
    public async Task<IActionResult> AiImprove([FromBody] AiTextRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { error = "text required" });

        try
        {
            var result = await ollama.ImproveDescriptionAsync(req.Text);
            return Ok(new { result });
        }
        catch
        {
            return StatusCode(503, new { error = "AI unavailable — make sure Ollama is running." });
        }
    }

    /* ── AI: parse natural language into structured log entry ── */
    [HttpPost]
    public async Task<IActionResult> AiParse([FromBody] AiParseRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { error = "text required" });

        try
        {
            var result = await ollama.ParseLogEntryAsync(req.Text);
            return Ok(new
            {
                project     = result.Project,
                hours       = result.Hours,
                description = result.Description,
                tags        = result.Tags,
            });
        }
        catch
        {
            return StatusCode(503, new { error = "AI unavailable" });
        }
    }

    /* ── AI: daily summary ── */
    [HttpPost]
    public async Task<IActionResult> AiSummary()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var logs  = await db.LogEntries
            .Where(l => l.UserId == CurrentUserId && l.Date == today)
            .ToListAsync();

        if (logs.Count == 0)
            return BadRequest(new { error = "No logs for today yet." });

        try
        {
            var result = await ollama.GenerateSummaryAsync(logs);
            return Ok(new { result });
        }
        catch
        {
            return StatusCode(503, new { error = "AI unavailable — make sure Ollama is running." });
        }
    }

    public record AiTextRequest(string Text);
    public record AiParseRequest(string Text);
}
