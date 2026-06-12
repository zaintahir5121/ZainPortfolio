using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class LearningController(AppDbContext db) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index(string? status)
    {
        var query = db.LearningItems.Where(l => l.UserId == UserId);

        if (!string.IsNullOrWhiteSpace(status) && status != "all")
            query = query.Where(l => l.Status == status);

        var items = await query
            .OrderBy(l => l.Status == "completed" ? 2 : l.Status == "in-progress" ? 0 : 1)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync();

        var all = await db.LearningItems.Where(l => l.UserId == UserId).ToListAsync();
        ViewBag.Status    = status ?? "all";
        ViewBag.InProgress = all.Count(l => l.Status == "in-progress");
        ViewBag.Completed  = all.Count(l => l.Status == "completed");
        ViewBag.WantCount  = all.Count(l => l.Status == "want");
        await SetNavStats();
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string title, string type, string source,
        string status, string notes, DateOnly? startedDate, DateOnly? completedDate, int rating)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            TempData["Error"] = "Title is required.";
            return RedirectToAction(nameof(Index));
        }

        db.LearningItems.Add(new LearningItem
        {
            UserId        = UserId,
            Title         = title.Trim(),
            Type          = type ?? "course",
            Source        = source?.Trim() ?? "",
            Status        = status ?? "in-progress",
            Notes         = notes?.Trim() ?? "",
            StartedDate   = startedDate,
            CompletedDate = completedDate,
            Rating        = Math.Clamp(rating, 0, 5),
            CreatedAt     = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
        TempData["Toast"] = "Added to your learning list!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var item = await db.LearningItems.FindAsync(id);
        if (item is null || item.UserId != UserId) return NotFound();

        item.Status = status;
        if (status == "completed" && item.CompletedDate is null)
            item.CompletedDate = DateOnly.FromDateTime(DateTime.Today);

        await db.SaveChangesAsync();
        TempData["Toast"] = status == "completed" ? "Marked as complete! 🎉" : "Status updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.LearningItems.FindAsync(id);
        if (item is null || item.UserId != UserId) return NotFound();

        db.LearningItems.Remove(item);
        await db.SaveChangesAsync();
        TempData["Toast"] = "Removed.";
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
