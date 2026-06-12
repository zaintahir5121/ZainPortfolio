using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class AchievementsController(AppDbContext db) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index(string? cat)
    {
        var query = db.Achievements.Where(a => a.UserId == UserId);

        if (!string.IsNullOrWhiteSpace(cat) && cat != "all")
            query = query.Where(a => a.Category == cat);

        var items = await query.OrderByDescending(a => a.Date).ToListAsync();

        ViewBag.Cat   = cat ?? "all";
        ViewBag.Total = await db.Achievements.CountAsync(a => a.UserId == UserId);
        await SetNavStats();
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string title, string description, DateOnly date, string category)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            TempData["Error"] = "Title is required.";
            return RedirectToAction(nameof(Index));
        }

        db.Achievements.Add(new Achievement
        {
            UserId      = UserId,
            Title       = title.Trim(),
            Description = description?.Trim() ?? "",
            Date        = date == default ? DateOnly.FromDateTime(DateTime.Today) : date,
            Category    = category ?? "milestone",
            CreatedAt   = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
        TempData["Toast"] = "Achievement added!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Achievements.FindAsync(id);
        if (item is null || item.UserId != UserId) return NotFound();

        db.Achievements.Remove(item);
        await db.SaveChangesAsync();
        TempData["Toast"] = "Deleted.";
        return RedirectToAction(nameof(Index));
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
}
