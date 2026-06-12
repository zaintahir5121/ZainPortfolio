using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class ExperienceController(AppDbContext db) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await db.Experiences
            .Where(e => e.UserId == UserId)
            .OrderByDescending(e => e.IsCurrent)
            .ThenByDescending(e => e.StartDate)
            .ToListAsync();

        await SetNavStats();
        return View(items);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(string company, string role, string location,
        DateOnly startDate, DateOnly? endDate, bool isCurrent, string description, string tags)
    {
        if (string.IsNullOrWhiteSpace(company) || string.IsNullOrWhiteSpace(role))
        {
            TempData["Error"] = "Company and role are required.";
            return RedirectToAction(nameof(Index));
        }

        db.Experiences.Add(new Experience
        {
            UserId      = UserId,
            Company     = company.Trim(),
            Role        = role.Trim(),
            Location    = location?.Trim() ?? "",
            StartDate   = startDate == default ? DateOnly.FromDateTime(DateTime.Today) : startDate,
            EndDate     = isCurrent ? null : endDate,
            IsCurrent   = isCurrent,
            Description = description?.Trim() ?? "",
            Tags        = tags?.Trim() ?? "",
            CreatedAt   = DateTime.UtcNow,
        });

        await db.SaveChangesAsync();
        TempData["Toast"] = "Experience added!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await db.Experiences.FindAsync(id);
        if (item is null || item.UserId != UserId) return NotFound();

        db.Experiences.Remove(item);
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
