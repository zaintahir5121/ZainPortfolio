using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;

namespace ZainPortfolio.Controllers;

[Route("subscribe")]
public class SubscribeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SubscribeController> _logger;

    public SubscribeController(ApplicationDbContext db, ILogger<SubscribeController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>AJAX endpoint used by the newsletter forms on the home page, blog and footer.</summary>
    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(string email, string? name, string? source, string? website)
    {
        // Honeypot — real users never fill a hidden field.
        if (!string.IsNullOrWhiteSpace(website))
            return Json(new { ok = true, message = "Thank you for subscribing." });

        if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            return Json(new { ok = false, message = "Please enter a valid email address." });

        email = email.Trim().ToLowerInvariant();

        var existing = await _db.Subscribers.FirstOrDefaultAsync(s => s.Email == email);
        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                existing.UnsubscribedAt = null;
                existing.SubscribedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                return Json(new { ok = true, message = "Welcome back — your subscription is active again." });
            }
            return Json(new { ok = true, message = "You're already subscribed — thank you." });
        }

        _db.Subscribers.Add(new Subscriber
        {
            Email = email,
            Name = string.IsNullOrWhiteSpace(name) ? null : name.Trim(),
            Source = string.IsNullOrWhiteSpace(source) ? "site" : source.Trim(),
            IsActive = true,
            IsConfirmed = true,      // single opt-in; switch to false if you add a confirmation mailer
            ConfirmToken = Guid.NewGuid().ToString("n"),
            IpHash = HashIp(HttpContext.Connection.RemoteIpAddress?.ToString()),
            SubscribedAt = DateTime.UtcNow,
            ConfirmedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        _logger.LogInformation("New subscriber from {Source}", source);

        return Json(new { ok = true, message = "You're subscribed — thank you. New posts will land in your inbox." });
    }

    [HttpGet("unsubscribe/{token}")]
    public async Task<IActionResult> Unsubscribe(string token)
    {
        var sub = await _db.Subscribers.FirstOrDefaultAsync(s => s.ConfirmToken == token);
        if (sub is not null)
        {
            sub.IsActive = false;
            sub.UnsubscribedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
        ViewBag.Found = sub is not null;
        return View();
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email.Trim());
            return addr.Address == email.Trim() && email.Contains('.');
        }
        catch { return false; }
    }

    private static string? HashIp(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return null;
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("zp-sub::" + ip)))[..32];
    }
}
