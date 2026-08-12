using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Services;

namespace ZainPortfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[Route("admin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAnalyticsService _analytics;

    public DashboardController(ApplicationDbContext db, IAnalyticsService analytics)
    {
        _db = db;
        _analytics = analytics;
    }

    [HttpGet("")]
    [HttpGet("dashboard")]
    public async Task<IActionResult> Index(int days = 30)
    {
        days = days is 7 or 14 or 30 or 90 ? days : 30;

        ViewBag.Days = days;
        ViewBag.Summary = await _analytics.GetSummaryAsync(days);
        ViewBag.ProjectCount = await _db.Projects.CountAsync();
        ViewBag.PublishedProjects = await _db.Projects.CountAsync(p => p.IsPublished);
        ViewBag.PostCount = await _db.BlogPosts.CountAsync();
        ViewBag.PublishedPosts = await _db.BlogPosts.CountAsync(p => p.IsPublished);
        ViewBag.MediaCount = await _db.MediaAssets.CountAsync();
        ViewBag.RecentMessages = await _db.ContactMessages.AsNoTracking()
            .OrderByDescending(m => m.CreatedAt).Take(5).ToListAsync();
        ViewBag.RecentSubscribers = await _db.Subscribers.AsNoTracking()
            .Where(s => s.IsActive)
            .OrderByDescending(s => s.SubscribedAt).Take(6).ToListAsync();

        return View();
    }

    [HttpGet("subscribers")]
    public async Task<IActionResult> Subscribers(string? q, int page = 1)
    {
        const int pageSize = 40;
        var query = _db.Subscribers.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(s => s.Email.Contains(q) || (s.Name != null && s.Name.Contains(q)));

        var total = await query.CountAsync();
        ViewBag.Total = total;
        ViewBag.ActiveCount = await _db.Subscribers.CountAsync(s => s.IsActive);
        ViewBag.Page = Math.Max(1, page);
        ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        ViewBag.Query = q;

        var items = await query.OrderByDescending(s => s.SubscribedAt)
            .Skip((Math.Max(1, page) - 1) * pageSize).Take(pageSize).ToListAsync();

        return View(items);
    }

    [HttpPost("subscribers/{id:int}/toggle")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSubscriber(int id)
    {
        var sub = await _db.Subscribers.FindAsync(id);
        if (sub is not null)
        {
            sub.IsActive = !sub.IsActive;
            sub.UnsubscribedAt = sub.IsActive ? null : DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Subscribers));
    }

    [HttpPost("subscribers/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSubscriber(int id)
    {
        var sub = await _db.Subscribers.FindAsync(id);
        if (sub is not null)
        {
            _db.Subscribers.Remove(sub);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Subscriber removed.";
        }
        return RedirectToAction(nameof(Subscribers));
    }

    /// <summary>CSV export so the list can be pushed into a mail platform.</summary>
    [HttpGet("subscribers/export")]
    public async Task<IActionResult> ExportSubscribers()
    {
        var rows = await _db.Subscribers.AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.SubscribedAt)
            .Select(s => new { s.Email, s.Name, s.Source, s.SubscribedAt })
            .ToListAsync();

        var sb = new System.Text.StringBuilder("Email,Name,Source,SubscribedAt\n");
        foreach (var r in rows)
            sb.AppendLine($"\"{r.Email}\",\"{r.Name}\",\"{r.Source}\",\"{r.SubscribedAt:yyyy-MM-dd HH:mm}\"");

        return File(System.Text.Encoding.UTF8.GetBytes(sb.ToString()),
            "text/csv", $"subscribers-{DateTime.UtcNow:yyyyMMdd}.csv");
    }

    [HttpGet("messages")]
    public async Task<IActionResult> Messages()
    {
        var items = await _db.ContactMessages.AsNoTracking()
            .OrderByDescending(m => m.CreatedAt).ToListAsync();
        ViewBag.UnreadCount = items.Count(m => !m.IsRead);
        return View(items);
    }

    [HttpPost("messages/{id:int}/read")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        var msg = await _db.ContactMessages.FindAsync(id);
        if (msg is not null)
        {
            msg.IsRead = !msg.IsRead;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Messages));
    }

    [HttpPost("messages/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        var msg = await _db.ContactMessages.FindAsync(id);
        if (msg is not null)
        {
            _db.ContactMessages.Remove(msg);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Message deleted.";
        }
        return RedirectToAction(nameof(Messages));
    }
}
