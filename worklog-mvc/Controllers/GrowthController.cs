using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class GrowthController(AppDbContext db) : Controller
{
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var uid = UserId;

        var achievements = await db.Achievements
            .Where(a => a.UserId == uid)
            .OrderByDescending(a => a.Date)
            .ToListAsync();

        var experiences = await db.Experiences
            .Where(e => e.UserId == uid)
            .OrderByDescending(e => e.IsCurrent)
            .ThenByDescending(e => e.StartDate)
            .ToListAsync();

        var learning = await db.LearningItems
            .Where(l => l.UserId == uid)
            .OrderBy(l => l.Status == "completed" ? 2 : l.Status == "in-progress" ? 0 : 1)
            .ThenByDescending(l => l.CreatedAt)
            .ToListAsync();

        ViewBag.AchieveCount = achievements.Count;
        ViewBag.XpCount      = experiences.Count;
        ViewBag.LearnCount   = learning.Count;
        ViewBag.LearnDone    = learning.Count(l => l.Status == "completed");
        ViewBag.Achievements = achievements;
        ViewBag.Experiences  = experiences;
        ViewBag.Learning     = learning;

        await SetNavStats();
        return View();
    }

    // ── Achievements ──────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddAchievement(string title, string description,
        DateOnly date, string category)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            db.Achievements.Add(new Achievement {
                UserId      = UserId,
                Title       = title.Trim(),
                Description = description?.Trim() ?? "",
                Date        = date == default ? DateOnly.FromDateTime(DateTime.Today) : date,
                Category    = category ?? "work",
                CreatedAt   = DateTime.UtcNow,
            });
            await db.SaveChangesAsync();
            TempData["Toast"] = "Achievement added! 🏆";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAchievement(int id)
    {
        var item = await db.Achievements.FindAsync(id);
        if (item != null && item.UserId == UserId)
        {
            db.Achievements.Remove(item);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // ── Experience ────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExperience(string company, string role,
        string location, string description, string tags,
        DateOnly startDate, DateOnly? endDate, bool isCurrent)
    {
        if (!string.IsNullOrWhiteSpace(company) && !string.IsNullOrWhiteSpace(role))
        {
            db.Experiences.Add(new Experience {
                UserId      = UserId,
                Company     = company.Trim(),
                Role        = role.Trim(),
                Location    = location?.Trim() ?? "",
                Description = description?.Trim() ?? "",
                Tags        = tags?.Trim() ?? "",
                StartDate   = startDate == default ? DateOnly.FromDateTime(DateTime.Today) : startDate,
                EndDate     = isCurrent ? null : endDate,
                IsCurrent   = isCurrent,
                CreatedAt   = DateTime.UtcNow,
            });
            await db.SaveChangesAsync();
            TempData["Toast"] = "Role added! 💼";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExperience(int id)
    {
        var item = await db.Experiences.FindAsync(id);
        if (item != null && item.UserId == UserId)
        {
            db.Experiences.Remove(item);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // ── Learning ──────────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AddLearning(string title, string type,
        string source, string status, string notes,
        DateOnly? startedDate, DateOnly? completedDate, int rating)
    {
        if (!string.IsNullOrWhiteSpace(title))
        {
            db.LearningItems.Add(new LearningItem {
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
            TempData["Toast"] = "Added to learning list!";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateLearningStatus(int id, string status)
    {
        var item = await db.LearningItems.FindAsync(id);
        if (item != null && item.UserId == UserId)
        {
            item.Status = status;
            if (status == "completed" && item.CompletedDate is null)
                item.CompletedDate = DateOnly.FromDateTime(DateTime.Today);
            await db.SaveChangesAsync();
            TempData["Toast"] = status == "completed" ? "Marked complete! 🎉" : "Status updated.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLearning(int id)
    {
        var item = await db.LearningItems.FindAsync(id);
        if (item != null && item.UserId == UserId)
        {
            db.LearningItems.Remove(item);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // ─────────────────────────────────────────────────────────────────────────

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
