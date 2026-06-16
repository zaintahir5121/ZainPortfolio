using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Services;

namespace WorkLogApp.Controllers;

[Authorize]
public class AssistantController(AppDbContext db, OllamaService ai) : Controller
{
    private int Uid => int.Parse(User.FindFirst("UserId")!.Value);

    // POST /Assistant/Chat
    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatReq req, CancellationToken ct)
    {
        try
        {
            var notes = await db.Notes
                .Where(n => n.UserId == Uid && !n.IsDeleted && !n.IsArchived)
                .OrderByDescending(n => n.UpdatedAt)
                .Take(30)
                .Select(n => new { n.Title, n.Content, n.IsPinned, n.UpdatedAt })
                .ToListAsync(ct);

            var noteLines = notes.Select(n =>
                $"- {(n.IsPinned ? "[pinned] " : "")}{n.Title}: {n.Content}".Trim());
            var notesBlock = string.Join("\n", noteLines);

            var system = $"""
                You are WorkLog AI, a smart assistant built into WorkLog, a work journal and timesheet app.
                The user's notes (most recent first):
                {notesBlock}

                Be concise, helpful and warm. Today: {DateTime.UtcNow:dddd, MMMM d, yyyy}.
                If the user asks about their notes, reference them. Keep replies short.
                """;

            var history = req.History?.Select(m => (m.Role, m.Content)) ?? [];
            var reply = await ai.ChatAsync(system, history, req.Message, ct);
            return Json(new { reply });
        }
        catch (Exception ex)
        {
            return Json(new { reply = $"⚠️ AI unavailable: {ex.Message.Split('\n')[0]}" });
        }
    }

    // POST /Assistant/DailySummary
    [HttpPost]
    public async Task<IActionResult> DailySummary(CancellationToken ct)
    {
        try
        {
            var today = DateTime.UtcNow.Date;
            var notes = await db.Notes
                .Where(n => n.UserId == Uid && !n.IsDeleted && n.UpdatedAt >= today)
                .OrderByDescending(n => n.UpdatedAt)
                .ToListAsync(ct);

            if (!notes.Any())
                return Json(new { summary = "No notes today yet — start capturing your thoughts! ✨" });

            var content = string.Join("\n\n", notes.Select(n =>
                string.IsNullOrEmpty(n.Title) ? n.Content : $"**{n.Title}**\n{n.Content}"));

            var system = """
                You are a smart productivity assistant. Create a brief, encouraging daily digest.
                Use 2-3 short sections with emoji headers. Be warm and actionable. Markdown output.
                """;
            var summary = await ai.ChatAsync(system, [],
                $"Today's notes to summarise:\n\n{content}", ct);
            return Json(new { summary });
        }
        catch (Exception ex)
        {
            return Json(new { summary = $"⚠️ AI unavailable: {ex.Message.Split('\n')[0]}" });
        }
    }

    // POST /Assistant/Polish
    [HttpPost]
    public async Task<IActionResult> Polish([FromBody] PolishReq req, CancellationToken ct)
    {
        try
        {
            var system = """
                You are a writing assistant. Improve the note text: fix grammar, improve clarity,
                and make it more useful — but keep the original meaning and length roughly the same.
                Return ONLY the improved text, no preamble.
                """;
            var improved = await ai.ChatAsync(system, [],
                $"Improve this note:\n\nTitle: {req.Title}\nContent: {req.Content}", ct);
            return Json(new { improved });
        }
        catch (Exception ex)
        {
            return Json(new { improved = (string?)null, error = ex.Message.Split('\n')[0] });
        }
    }

    // POST /Assistant/Standup
    [HttpPost]
    public async Task<IActionResult> Standup(CancellationToken ct)
    {
        try
        {
            var yesterday = DateTime.UtcNow.Date.AddDays(-1);
            var tasks = await db.WorkTasks.Include(t => t.WorkEntry)
                .Where(t => t.WorkEntry.UserId == Uid && t.WorkEntry.LogDate == yesterday)
                .ToListAsync(ct);
            if (!tasks.Any())
                return Json(new { standup = "No work logged for yesterday. Start logging with the Work Log!" });
            var content = string.Join("\n", tasks.Select(t =>
                $"- {t.Description} ({t.Hours}h){(t.Project != null ? " [" + t.Project + "]" : "")}"));
            var sys = "You are a daily standup assistant. Write a concise, professional standup update (3 bullet points max) from the work done. Format: **Yesterday:** bullet points. Be brief and professional.";
            var standup = await ai.ChatAsync(sys, [], $"Work done yesterday:\n{content}", ct);
            return Json(new { standup });
        }
        catch (Exception ex)
        {
            return Json(new { standup = $"⚠️ AI unavailable: {ex.Message.Split('\n')[0]}" });
        }
    }
}

public record ChatReq(string Message, List<ChatMsg>? History);
public record ChatMsg(string Role, string Content);
public record PolishReq(string? Title, string? Content);
