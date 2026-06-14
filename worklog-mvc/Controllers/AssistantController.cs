using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Services;

namespace WorkLogApp.Controllers;

[Authorize]
public class AssistantController(AppDbContext db, IOllamaService ollama) : Controller
{
    private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var weekStart = today.AddDays(-(int)today.DayOfWeek == 0 ? -6 : -(int)today.DayOfWeek + 1);

        var recentLogs = await db.LogEntries
            .Where(l => l.UserId == CurrentUserId && l.Date >= weekStart)
            .OrderByDescending(l => l.Date)
            .Take(20)
            .ToListAsync();

        ViewBag.TodayHours = recentLogs.Where(l => l.Date == today).Sum(l => l.Hours);
        ViewBag.FirstName  = (User.FindFirstValue(ClaimTypes.Name) ?? "there").Split(' ')[0];
        ViewBag.TodayCount = recentLogs.Count(l => l.Date == today);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] AiChatRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Message))
            return BadRequest(new { error = "Message required" });

        var today      = DateOnly.FromDateTime(DateTime.Today);
        var weekStart  = today.AddDays(-(int)today.DayOfWeek == 0 ? -6 : -(int)today.DayOfWeek + 1);
        var recentLogs = await db.LogEntries
            .Where(l => l.UserId == CurrentUserId && l.Date >= weekStart)
            .OrderByDescending(l => l.Date)
            .Take(20)
            .ToListAsync();

        string context = "";
        if (recentLogs.Any())
        {
            var lines = recentLogs.Select(l =>
                $"• {l.Date:ddd MMM d}: [{l.Tags}] {l.Description} ({l.Hours}h){(string.IsNullOrWhiteSpace(l.Project) ? "" : " — " + l.Project)}");
            var total = recentLogs.Sum(l => l.Hours);
            var todayH = recentLogs.Where(l => l.Date == today).Sum(l => l.Hours);
            context = $"This week: {total:F1}h total, {todayH:F1}h today\n{string.Join('\n', lines)}";
        }

        try
        {
            var response = await ollama.ChatAsync(req.Message.Trim(), context);
            return Ok(new { response });
        }
        catch (Exception ex)
        {
            var hint = ex.Message.Contains("connect") || ex.Message.Contains("refused") || ex.Message.Contains("timeout")
                ? "Ollama is not running. Start it with: ollama serve"
                : "AI service unavailable right now.";
            return StatusCode(503, new { error = hint });
        }
    }

    public record AiChatRequest(string Message);
}
