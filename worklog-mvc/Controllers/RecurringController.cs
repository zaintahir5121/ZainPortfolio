using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class RecurringController(AppDbContext db) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var entries = await db.RecurringEntries
            .Where(r => r.UserId == UserId)
            .OrderByDescending(r => r.IsActive).ThenBy(r => r.CreatedAt)
            .ToListAsync();

        await SetNavStats();
        return View(entries);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(RecurringEntry model)
    {
        if (string.IsNullOrWhiteSpace(model.Project) || model.Hours <= 0)
        {
            TempData["Error"] = "Project and hours are required.";
            return RedirectToAction(nameof(Index));
        }

        model.UserId    = UserId;
        model.IsActive  = true;
        model.CreatedAt = DateTime.UtcNow;
        model.Schedule  = Normalize(model.Schedule);

        db.RecurringEntries.Add(model);
        await db.SaveChangesAsync();

        TempData["Toast"] = $"↺ Recurring entry set — fires {model.ScheduleLabel.ToLower()}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int id)
    {
        var entry = await db.RecurringEntries.FindAsync(id);
        if (entry is null || entry.UserId != UserId) return NotFound();

        entry.IsActive = !entry.IsActive;
        await db.SaveChangesAsync();

        TempData["Toast"] = entry.IsActive ? "↺ Entry enabled" : "Entry paused";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await db.RecurringEntries.FindAsync(id);
        if (entry is null || entry.UserId != UserId) return NotFound();

        db.RecurringEntries.Remove(entry);
        await db.SaveChangesAsync();

        TempData["Toast"] = "Recurring entry deleted.";
        return RedirectToAction(nameof(Index));
    }

    /* ── Called from LogsController on every dashboard visit ── */
    public static async Task<int> FireTodayAsync(AppDbContext db, int userId)
    {
        var entries = await db.RecurringEntries
            .Where(r => r.UserId == userId && r.IsActive)
            .ToListAsync();

        var today = DateOnly.FromDateTime(DateTime.Today);
        var fired = 0;

        foreach (var r in entries)
        {
            if (!r.ShouldFireToday()) continue;

            db.LogEntries.Add(new LogEntry
            {
                UserId      = userId,
                Date        = today,
                Project     = r.Project,
                Description = r.Description,
                Hours       = r.Hours,
                Tags        = r.Tags,
                CreatedAt   = DateTime.UtcNow,
            });

            r.LastFiredDate = today;
            fired++;
        }

        if (fired > 0) await db.SaveChangesAsync();
        return fired;
    }

    private async Task SetNavStats()
    {
        var today     = DateOnly.FromDateTime(DateTime.Today);
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var all       = await db.LogEntries.Where(l => l.UserId == UserId).ToListAsync();
        ViewBag.TodayHours    = all.Where(l => l.Date == today).Sum(l => l.Hours);
        ViewBag.WeekLogsCount = all.Count(l => l.Date >= weekStart);
        ViewBag.ProjectsCount = all.Select(l => l.Project).Distinct().Count();
    }

    private static string Normalize(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return "weekdays";
        s = s.Trim().ToLower();
        if (s is "daily" or "weekdays") return s;
        var days = s.Split(',')
                    .Select(d => d.Trim())
                    .Where(d => d.Length >= 3)
                    .Select(d => d[..3])
                    .ToList();
        return days.Count > 0 ? string.Join(",", days) : "weekdays";
    }
}
