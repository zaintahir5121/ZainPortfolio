using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class FeedbackController(AppDbContext db) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] FeedbackRequest req)
    {
        var userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? (int?)id : null;
        var name   = User.FindFirstValue(ClaimTypes.Name) ?? "";

        db.Feedbacks.Add(new FeedbackEntry
        {
            UserId    = userId,
            UserName  = name,
            Rating    = Math.Clamp(req.Rating, 1, 5),
            Category  = (req.Category ?? "General Feedback").Trim(),
            Message   = (req.Message ?? "").Trim(),
            CreatedAt = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();
        return Ok(new { ok = true });
    }

    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var items = await db.Feedbacks
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
        return View(items);
    }

    public record FeedbackRequest(int Rating, string? Category, string? Message);
}
