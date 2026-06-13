using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;
using WorkLogApp.Services;

namespace WorkLogApp.Controllers;

/// <summary>
/// Public webhook endpoint — no cookie auth required, secured by API key.
/// Used by Microsoft Teams (Power Automate), Slack, or any HTTP client.
///
/// POST /api/webhook/log
///   Body: { "apiKey": "YOUR_KEY", "text": "Fixed auth bug 2h, standup 15min" }
///   Returns: { "logged": 2, "entries": [...] }
/// </summary>
[ApiController]
[Route("api/webhook")]
public class WebhookController(AppDbContext db, IOllamaService ollama) : ControllerBase
{
    [HttpPost("log")]
    public async Task<IActionResult> Log([FromBody] WebhookLogRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.ApiKey) || string.IsNullOrWhiteSpace(req.Text))
            return BadRequest(new { error = "apiKey and text are required" });

        var user = await db.Users.FirstOrDefaultAsync(u => u.ApiKey == req.ApiKey);
        if (user is null)
            return Unauthorized(new { error = "Invalid API key" });

        List<ParsedDayEntry> parsed;
        try
        {
            parsed = await ollama.ParseDayLogAsync(req.Text);
            if (parsed.Count == 0) throw new Exception("empty");
        }
        catch
        {
            parsed = [new ParsedDayEntry(req.Text.Trim(), 1, "Other", "General")];
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var saved = new List<LogEntry>();

        foreach (var e in parsed.Where(e => !string.IsNullOrWhiteSpace(e.Description)))
        {
            var entry = new LogEntry
            {
                UserId      = user.Id,
                Date        = today,
                Project     = !string.IsNullOrWhiteSpace(e.Project) ? e.Project.Trim() : (e.Category ?? "General"),
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
            logged  = saved.Count,
            total   = saved.Sum(e => e.Hours),
            entries = saved.Select(e => new { e.Description, e.Project, e.Hours, category = e.Tags }),
            user    = user.Name,
        });
    }

    [HttpGet("ping")]
    public IActionResult Ping() => Ok(new { status = "ok", time = DateTime.UtcNow });
}

public record WebhookLogRequest(string ApiKey, string Text);
