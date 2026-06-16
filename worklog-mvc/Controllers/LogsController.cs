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
        var insightMonth  = new DateOnly(today.Year, today.Month, 1);

        var weekHours     = stats.Where(l => l.Date >= weekStart && l.Date <= today).Sum(l => l.Hours);
        var lastWeekHours = stats.Where(l => l.Date >= lastWeekStart && l.Date <= lastWeekEnd).Sum(l => l.Hours);

        var topProj = stats
            .Where(l => l.Date >= insightMonth)
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

        // ── Weekly day breakdown (Mon–Sun) for sidebar ───────────────────────
        var dow         = (int)today.DayOfWeek;
        var monStart    = today.AddDays(dow == 0 ? -6 : -(dow - 1));
        var weekDays    = Enumerable.Range(0, 7).Select(i =>
        {
            var d = monStart.AddDays(i);
            return new WeekDayStat(d.ToString("ddd"), stats.Where(l => l.Date == d).Sum(l => l.Hours), d == today, d > today);
        }).ToList();

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
            WeekHours        = weekDays.Sum(d => d.Hours),
            LastWeekHours    = lastWeekHours,
            TopProject       = topProj?.Name ?? "",
            TopProjectHours  = topProj?.Hours ?? 0,
            WarnNotLogged    = warnNotLogged,
            RecentProjects   = recentProjects,
            WeekDays         = weekDays,
            WeekGoal         = 40m,
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
            return StatusCode(503, new { error = "AI unavailable — please try again shortly." });
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
            return StatusCode(503, new { error = "AI unavailable — please try again shortly." });
        }
    }

    /* ── AI: parse full day description into multiple entries ── */
    [HttpPost]
    public async Task<IActionResult> ParseDay([FromBody] AiTextRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { error = "text required" });

        try
        {
            var entries = await ollama.ParseDayLogAsync(req.Text);
            return Ok(entries);
        }
        catch
        {
            return StatusCode(503, new { error = "AI unavailable — please try again shortly." });
        }
    }

    /* ── Bulk-save a day's parsed entries ── */
    [HttpPost]
    public async Task<IActionResult> BulkAdd([FromBody] BulkAddRequest req)
    {
        if (req.Entries is null || req.Entries.Count == 0)
            return BadRequest(new { error = "no entries" });

        var today = req.Date ?? DateOnly.FromDateTime(DateTime.Today);
        foreach (var e in req.Entries.Where(e => !string.IsNullOrWhiteSpace(e.Description)))
        {
            db.LogEntries.Add(new LogEntry
            {
                UserId      = CurrentUserId,
                Date        = today,
                Project     = (e.Project?.Trim().Length > 0 ? e.Project : e.Category) ?? "General",
                Description = e.Description.Trim(),
                Hours       = e.Hours,
                Tags        = e.Category?.Trim() ?? "",
                CreatedAt   = DateTime.UtcNow,
            });
        }

        await db.SaveChangesAsync();
        return Ok(new { saved = req.Entries.Count });
    }

    /* ── Chat: parse + save in one shot ── */
    [HttpPost]
    public async Task<IActionResult> ChatLog([FromBody] ChatLogRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { error = "text required" });

        List<ParsedDayEntry> parsed;
        bool usedAi = true;
        try
        {
            parsed = await ollama.ParseDayLogAsync(req.Text);
            if (parsed.Count == 0) throw new Exception("empty");
        }
        catch
        {
            usedAi = false;
            parsed = [new ParsedDayEntry(req.Text.Trim(), 1, "Other", "General")];
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var saved = new List<LogEntry>();

        foreach (var e in parsed.Where(e => !string.IsNullOrWhiteSpace(e.Description)))
        {
            var entry = new LogEntry
            {
                UserId      = CurrentUserId,
                Date        = today,
                Project     = !string.IsNullOrWhiteSpace(req.ProjectOverride)
                    ? req.ProjectOverride.Trim()
                    : (!string.IsNullOrWhiteSpace(e.Project) ? e.Project.Trim() : (e.Category ?? "General")),
                Description = e.Description.Trim(),
                Hours       = e.Hours > 0 ? e.Hours : 1,
                Tags        = e.Category?.Trim() ?? "",
                CreatedAt   = DateTime.UtcNow,
            };
            db.LogEntries.Add(entry);
            saved.Add(entry);
        }
        await db.SaveChangesAsync();

        return Ok(new
        {
            entries = saved.Select(e => new
            {
                id          = e.Id,
                description = e.Description,
                project     = e.Project,
                hours       = e.Hours,
                category    = e.Tags,
            }),
            total  = saved.Sum(e => e.Hours),
            usedAi,
        });
    }

    /* ── Weekly AI insight (data-driven) ── */
    [HttpGet]
    public async Task<IActionResult> WeeklyInsight()
    {
        var uid   = CurrentUserId;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var dow   = (int)today.DayOfWeek;
        var monW  = today.AddDays(dow == 0 ? -6 : -(dow - 1));
        var monLW = monW.AddDays(-7);

        var all = await db.LogEntries.Where(l => l.UserId == uid).ToListAsync();

        var thisWeek = all.Where(l => l.Date >= monW && l.Date <= today).ToList();
        var lastWeek = all.Where(l => l.Date >= monLW && l.Date < monW).ToList();

        if (thisWeek.Count == 0 && lastWeek.Count == 0)
            return Ok(new { insight = "Start logging your work and I'll surface patterns here each week. The more you log, the smarter the insights." });

        var parts = new List<string>();

        // Meeting % this week vs last
        var thisMtg  = thisWeek.Where(l => l.Tags == "Meeting").Sum(l => l.Hours);
        var thisTotal = thisWeek.Sum(l => l.Hours);
        var lastTotal = lastWeek.Sum(l => l.Hours);
        if (thisTotal > 0)
        {
            var mtgPct = (int)Math.Round(thisMtg / thisTotal * 100);
            if (lastTotal > 0)
            {
                var lastMtg    = lastWeek.Where(l => l.Tags == "Meeting").Sum(l => l.Hours);
                var lastMtgPct = (int)Math.Round(lastMtg / lastTotal * 100);
                var diff       = mtgPct - lastMtgPct;
                if (Math.Abs(diff) >= 10)
                    parts.Add($"You spent {mtgPct}% of your time in meetings this week — {(diff > 0 ? $"up {diff}% from last week" : $"down {Math.Abs(diff)}% from last week")}.");
                else if (mtgPct >= 40)
                    parts.Add($"Heads up — {mtgPct}% of your week went to meetings.");
            }
        }

        // Best output day
        var dayBest = thisWeek
            .GroupBy(l => l.Date)
            .Select(g => new { Day = g.Key, Hours = g.Sum(l => l.Hours) })
            .OrderByDescending(x => x.Hours)
            .FirstOrDefault();
        if (dayBest != null && thisWeek.Count > 1)
            parts.Add($"Your most productive day was {dayBest.Day.ToString("dddd")} with {dayBest.Hours}h logged.");

        // Projects gone silent (active last week, nothing this week)
        var thisProjects = new HashSet<string>(thisWeek.Select(l => l.Project));
        var silentProjs  = lastWeek.Select(l => l.Project)
            .Distinct()
            .Where(p => !string.IsNullOrWhiteSpace(p) && !thisProjects.Contains(p))
            .Take(2)
            .ToList();
        if (silentProjs.Count > 0)
            parts.Add($"You haven't touched {string.Join(" or ", silentProjs.Select(p => $"\"{p}\""))} this week — did it wrap up or get paused?");

        // Hours comparison
        if (thisTotal > 0 && lastTotal > 0)
        {
            var diff = thisTotal - lastTotal;
            if (Math.Abs(diff) >= 2)
                parts.Add($"You're on track for {(diff > 0 ? "more" : "fewer")} hours than last week ({thisTotal:F0}h vs {lastTotal:F0}h).");
        }

        var insight = parts.Count > 0
            ? string.Join(" ", parts)
            : $"You've logged {thisTotal}h across {thisWeek.Select(l => l.Project).Distinct().Count()} projects this week. Keep it up!";

        return Ok(new { insight });
    }

    /* ── Get logs for any date ── */
    [HttpGet]
    public async Task<IActionResult> DateLogs(string date)
    {
        if (!DateOnly.TryParse(date, out var d))
            return BadRequest(new { error = "Invalid date format. Use YYYY-MM-DD." });

        var logs = await db.LogEntries
            .Where(l => l.UserId == CurrentUserId && l.Date == d)
            .OrderBy(l => l.CreatedAt)
            .ToListAsync();

        return Ok(new
        {
            date    = d.ToString("dddd, MMMM d"),
            total   = logs.Sum(l => l.Hours),
            entries = logs.Select(l => new
            {
                l.Id,
                description = l.Description,
                project     = l.Project,
                hours       = l.Hours,
                category    = l.Tags,
            }),
        });
    }

    /* ── AI: fix English/grammar ── */
    [HttpPost]
    public async Task<IActionResult> FixText([FromBody] AiTextRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { error = "text required" });
        try
        {
            var result = await ollama.FixEnglishAsync(req.Text);
            return Ok(new { result });
        }
        catch
        {
            return Ok(new { result = req.Text }); // fallback: return unchanged
        }
    }

    /* ── AI: improve a task description ── */
    [HttpPost]
    public async Task<IActionResult> ImproveDescription([FromBody] AiTextRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Text)) return BadRequest(new { error = "text required" });
        try
        {
            var result = await ollama.ImproveDescriptionAsync(req.Text);
            return Ok(new { result });
        }
        catch
        {
            return Ok(new { result = req.Text });
        }
    }

    /* ── Edit a saved entry (AJAX) ── */
    [HttpPost]
    public async Task<IActionResult> EditLog([FromBody] EditLogRequest req)
    {
        var entry = await db.LogEntries.FindAsync(req.Id);
        if (entry is null) return NotFound(new { error = "Not found" });
        if (entry.UserId != CurrentUserId && !IsAdmin) return Forbid();

        if (!string.IsNullOrWhiteSpace(req.Description)) entry.Description = req.Description.Trim();
        if (req.Hours is > 0) entry.Hours = req.Hours.Value;
        if (req.Project != null) entry.Project = req.Project.Trim();
        if (req.Category != null) entry.Tags = req.Category.Trim();
        if (req.Date.HasValue) entry.Date = req.Date.Value;

        await db.SaveChangesAsync();
        return Ok(new { ok = true, hours = entry.Hours, description = entry.Description });
    }

    /* ── AJAX delete (card board) ── */
    [HttpPost]
    public async Task<IActionResult> DeleteEntry([FromBody] DeleteEntryRequest req)
    {
        var entry = await db.LogEntries.FindAsync(req.Id);
        if (entry is null) return NotFound(new { error = "Not found" });
        if (entry.UserId != CurrentUserId && !IsAdmin) return Forbid();
        db.LogEntries.Remove(entry);
        await db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    /* ── Widget inline edit ── */
    [HttpPost]
    public async Task<IActionResult> UpdateEntry([FromBody] UpdateEntryRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Description))
            return BadRequest(new { error = "description required" });
        var entry = await db.LogEntries.FindAsync(req.Id);
        if (entry is null) return NotFound(new { error = "Not found" });
        if (entry.UserId != CurrentUserId && !IsAdmin) return Forbid();
        entry.Description = req.Description.Trim();
        await db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    /* ── Sticky Notes Widget ── */
    [HttpGet]
    public async Task<IActionResult> Sticky()
    {
        var today     = DateOnly.FromDateTime(DateTime.Today);
        var yesterday = today.AddDays(-1);
        var allMine   = db.LogEntries.Where(l => l.UserId == CurrentUserId);
        var todayLogs = await allMine
            .Where(l => l.Date == today)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
        var yesterdayLogs = await allMine
            .Where(l => l.Date == yesterday)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
        var recentProjects = await allMine
            .GroupBy(l => l.Project)
            .Select(g => new { Name = g.Key, LastDate = g.Max(e => e.Date) })
            .OrderByDescending(x => x.LastDate)
            .Select(x => x.Name)
            .Take(10)
            .ToListAsync();
        ViewBag.TodayHours     = todayLogs.Sum(l => l.Hours);
        ViewBag.YesterdayLogs  = yesterdayLogs;
        ViewBag.YesterdayHours = yesterdayLogs.Sum(l => l.Hours);
        ViewBag.FirstName      = (User.FindFirstValue(ClaimTypes.Name) ?? "there").Split(' ')[0];
        ViewBag.RecentProjects = recentProjects;
        ViewBag.Date           = DateTime.Today.ToString("ddd, MMM d");
        return View(todayLogs);
    }

    /* ── Day-Preview Widget (moveable PWA standalone window) ── */
    [HttpGet]
    public async Task<IActionResult> Widget()
    {
        var today     = DateOnly.FromDateTime(DateTime.Today);
        var yesterday = today.AddDays(-1);
        var weekStart = today.AddDays(-6);
        var allMine   = db.LogEntries.Where(l => l.UserId == CurrentUserId);

        var todayLogs = await allMine
            .Where(l => l.Date == today)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        var yesterdayLogs = await allMine
            .Where(l => l.Date == yesterday)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

        // Fetch week raw entries in memory, then group (EF can't translate GroupBy+Select+OrderBy together)
        var weekEntries = await allMine
            .Where(l => l.Date >= weekStart && l.Date <= today)
            .Select(l => new { l.Date, l.Hours })
            .ToListAsync();
        var weekGrouped = weekEntries
            .GroupBy(l => l.Date)
            .Select(g => new { Date = g.Key, Hours = g.Sum(x => x.Hours) })
            .OrderBy(x => x.Date)
            .ToList();

        var recentProjects = await allMine
            .GroupBy(l => l.Project)
            .Select(g => new { Name = g.Key, LastDate = g.Max(e => e.Date) })
            .OrderByDescending(x => x.LastDate)
            .Select(x => x.Name)
            .Take(8)
            .ToListAsync();

        var todayWork  = todayLogs.Where(l => l.Tags != "Notes").ToList();
        var todayNotes = todayLogs.Where(l => l.Tags == "Notes").ToList();
        var ydayNotes  = yesterdayLogs.Where(l => l.Tags == "Notes").ToList();
        var ydayWork   = yesterdayLogs.Where(l => l.Tags != "Notes").ToList();

        ViewBag.TodayLogs      = todayWork;
        ViewBag.TodayNotes     = todayNotes;
        ViewBag.YesterdayLogs  = ydayWork;
        ViewBag.YesterdayNotes = ydayNotes;
        ViewBag.TodayHours     = todayWork.Sum(l => l.Hours);
        ViewBag.YesterdayHours = ydayWork.Sum(l => l.Hours);
        ViewBag.DailyGoal      = 8m;
        ViewBag.FirstName      = (User.FindFirstValue(ClaimTypes.Name) ?? "there").Split(' ')[0];
        ViewBag.Date           = DateTime.Today.ToString("ddd, MMM d");
        ViewBag.RecentProjects = recentProjects;
        ViewBag.WeekJson       = System.Text.Json.JsonSerializer.Serialize(
            weekGrouped.Select(x => new { d = x.Date.ToString("ddd")[..1], h = (double)x.Hours }));

        return View();
    }

    /* ── Auto-start setup script download ── */
    [HttpGet]
    public IActionResult SetupScript()
    {
        var origin = $"{Request.Scheme}://{Request.Host}";
        // Use verbatim string so PowerShell $ / { } are not interpreted by C#
        var script = @"# WorkLog Widget — Auto-Start Setup
# Right-click this file and choose ""Run with PowerShell""
# (or: powershell -ExecutionPolicy Bypass -File setup-worklog-widget.ps1)

$widgetUrl = ""__WIDGET_URL__""

$candidates = @(
  ""${env:ProgramFiles(x86)}\Microsoft\Edge\Application\msedge.exe"",
  ""$env:ProgramFiles\Microsoft\Edge\Application\msedge.exe"",
  ""$env:ProgramFiles\Google\Chrome\Application\chrome.exe"",
  ""${env:ProgramFiles(x86)}\Google\Chrome\Application\chrome.exe""
)
$browser = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1

if (-not $browser) {
  Write-Host ""Edge or Chrome not found. Please install one first."" -ForegroundColor Red
  Read-Host ""Press Enter to exit""; exit 1
}

$browserArgs = ""--app=$widgetUrl --window-size=400,660""
$startupDir  = [Environment]::GetFolderPath('Startup')
$desktopDir  = [Environment]::GetFolderPath('Desktop')

function Set-Lnk([string]$Dest, [string]$Tgt, [string]$Arg, [string]$Desc) {
  $wsh = New-Object -ComObject WScript.Shell
  $sc  = $wsh.CreateShortcut($Dest)
  $sc.TargetPath = $Tgt; $sc.Arguments = $Arg; $sc.Description = $Desc
  $sc.Save()
}

Set-Lnk ""$startupDir\WorkLog Widget.lnk"" $browser $browserArgs ""WorkLog Widget""
Set-Lnk ""$desktopDir\WorkLog Widget.lnk"" $browser $browserArgs ""WorkLog Widget""

Write-Host """"
Write-Host ""WorkLog Widget is set up!"" -ForegroundColor Green
Write-Host ""  Startup : $startupDir\WorkLog Widget.lnk""
Write-Host ""  Desktop : $desktopDir\WorkLog Widget.lnk""
Write-Host """"
Write-Host ""It will open automatically on next login."" -ForegroundColor Cyan
Read-Host ""Press Enter to close""
".Replace("__WIDGET_URL__", origin + "/Logs/Widget");
        return File(System.Text.Encoding.UTF8.GetBytes(script), "application/octet-stream", "setup-worklog-widget.ps1");
    }

    /* ── Quick add — instant save, no AI ── */
    [HttpPost]
    public async Task<IActionResult> QuickAdd([FromBody] QuickAddRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Description))
            return BadRequest(new { error = "description required" });

        var entry = new LogEntry
        {
            UserId      = CurrentUserId,
            Date        = DateOnly.FromDateTime(DateTime.Today),
            Description = req.Description.Trim(),
            Hours       = req.Hours > 0 ? req.Hours : 1,
            Tags        = string.IsNullOrWhiteSpace(req.Category) ? "Other" : req.Category.Trim(),
            Project     = string.IsNullOrWhiteSpace(req.Project)
                              ? (string.IsNullOrWhiteSpace(req.Category) ? "General" : req.Category.Trim())
                              : req.Project.Trim(),
            CreatedAt   = DateTime.UtcNow,
        };
        db.LogEntries.Add(entry);
        await db.SaveChangesAsync();
        return Ok(new { id = entry.Id, description = entry.Description, hours = entry.Hours, category = entry.Tags, project = entry.Project });
    }

    public record AiTextRequest(string Text);
    public record AiParseRequest(string Text);
    public record BulkAddEntry(string Description, decimal Hours, string Category, string Project);
    public record BulkAddRequest(List<BulkAddEntry> Entries, DateOnly? Date = null);
    public record EditLogRequest(int Id, string? Description, decimal? Hours, string? Project, string? Category, DateOnly? Date = null);
    public record DeleteEntryRequest(int Id);
    public record UpdateEntryRequest(int Id, string Description);
    public record ChatLogRequest(string Text, string? Date = null, string? ProjectOverride = null);
    public record QuickAddRequest(string Description, decimal Hours = 1, string? Category = null, string? Project = null);
}
