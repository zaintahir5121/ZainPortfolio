using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;

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
            .Take(4)
            .ToListAsync();

        var experiences = await db.Experiences
            .Where(e => e.UserId == uid)
            .OrderByDescending(e => e.IsCurrent)
            .ThenByDescending(e => e.StartDate)
            .Take(3)
            .ToListAsync();

        var learning = await db.LearningItems
            .Where(l => l.UserId == uid)
            .OrderByDescending(l => l.CreatedAt)
            .Take(4)
            .ToListAsync();

        var achieveCount = await db.Achievements.CountAsync(a => a.UserId == uid);
        var xpCount      = await db.Experiences.CountAsync(e => e.UserId == uid);
        var learnCount   = await db.LearningItems.CountAsync(l => l.UserId == uid);
        var learnDone    = await db.LearningItems.CountAsync(l => l.UserId == uid && l.Status == "completed");

        await SetNavStats();

        ViewBag.AchieveCount  = achieveCount;
        ViewBag.XpCount       = xpCount;
        ViewBag.LearnCount    = learnCount;
        ViewBag.LearnDone     = learnDone;
        ViewBag.Achievements  = achievements;
        ViewBag.Experiences   = experiences;
        ViewBag.Learning      = learning;

        return View();
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
