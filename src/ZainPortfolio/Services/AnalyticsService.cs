using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;

namespace ZainPortfolio.Services;

public record StatPoint(string Label, int Value);

public record AnalyticsSummary
{
    public int ViewsToday { get; init; }
    public int ViewsWeek { get; init; }
    public int ViewsMonth { get; init; }
    public int ViewsTotal { get; init; }
    public int VisitorsToday { get; init; }
    public int VisitorsWeek { get; init; }
    public int VisitorsMonth { get; init; }
    public int VisitorsTotal { get; init; }
    public int SubscribersTotal { get; init; }
    public int SubscribersMonth { get; init; }
    public int MessagesUnread { get; init; }
    public int MessagesTotal { get; init; }
    public List<StatPoint> DailyViews { get; init; } = new();
    public List<StatPoint> DailyVisitors { get; init; } = new();
    public List<StatPoint> TopPages { get; init; } = new();
    public List<StatPoint> TopReferrers { get; init; } = new();
    public List<StatPoint> Devices { get; init; } = new();
    public List<StatPoint> TopProjects { get; init; } = new();
    public List<StatPoint> TopPosts { get; init; } = new();
}

public interface IAnalyticsService
{
    Task TrackAsync(HttpContext ctx, string pageType, int? entityId, string? title);
    Task<AnalyticsSummary> GetSummaryAsync(int days = 30);
}

public class AnalyticsService : IAnalyticsService
{
    private const string VisitorCookie = "zp.vid";

    private readonly ApplicationDbContext _db;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(ApplicationDbContext db, ILogger<AnalyticsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    private static readonly string[] BotSignals =
    {
        "bot", "crawler", "spider", "slurp", "bingpreview", "headless", "lighthouse",
        "pingdom", "monitor", "curl", "wget", "python-requests", "postman", "playwright"
    };

    public async Task TrackAsync(HttpContext ctx, string pageType, int? entityId, string? title)
    {
        try
        {
            var ua = ctx.Request.Headers.UserAgent.ToString();
            if (string.IsNullOrWhiteSpace(ua)) return;

            var lower = ua.ToLowerInvariant();
            if (BotSignals.Any(lower.Contains)) return;      // never store bot traffic

            // Stable pseudonymous visitor id in a first-party cookie.
            var isNew = false;
            if (!ctx.Request.Cookies.TryGetValue(VisitorCookie, out var visitorId) ||
                string.IsNullOrWhiteSpace(visitorId))
            {
                visitorId = Guid.NewGuid().ToString("n");
                isNew = true;
                ctx.Response.Cookies.Append(VisitorCookie, visitorId, new CookieOptions
                {
                    HttpOnly = true,
                    IsEssential = false,
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            }

            var referrer = ctx.Request.Headers.Referer.ToString();
            string? refHost = null;
            if (!string.IsNullOrWhiteSpace(referrer) &&
                Uri.TryCreate(referrer, UriKind.Absolute, out var refUri))
            {
                refHost = refUri.Host;
                if (refHost.Equals(ctx.Request.Host.Host, StringComparison.OrdinalIgnoreCase))
                    refHost = null;      // internal navigation is not a referrer
            }

            _db.PageViews.Add(new PageView
            {
                Path = Truncate(ctx.Request.Path.Value ?? "/", 400) ?? "/",
                PageType = pageType,
                EntityId = entityId,
                Title = Truncate(title, 300),
                Referrer = Truncate(referrer, 400),
                ReferrerHost = Truncate(refHost, 60),
                UserAgent = Truncate(ua, 300),
                DeviceType = DeviceFrom(lower),
                VisitorId = visitorId,
                IpHash = HashIp(ctx.Connection.RemoteIpAddress?.ToString()),
                IsNewVisitor = isNew,
                CreatedAt = DateTime.UtcNow,
                Day = DateOnly.FromDateTime(DateTime.UtcNow)
            });
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Analytics must never break a page render.
            _logger.LogDebug(ex, "Page view tracking failed for {Path}", ctx.Request.Path);
        }
    }

    public async Task<AnalyticsSummary> GetSummaryAsync(int days = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var weekAgo = today.AddDays(-6);
        var monthAgo = today.AddDays(-29);
        var rangeStart = today.AddDays(-(days - 1));
        var monthStartUtc = DateTime.UtcNow.AddDays(-30);

        var views = _db.PageViews.AsNoTracking();

        var daily = await views.Where(v => v.Day >= rangeStart)
            .GroupBy(v => v.Day)
            .Select(g => new { Day = g.Key, Views = g.Count(), Visitors = g.Select(x => x.VisitorId).Distinct().Count() })
            .ToListAsync();

        var dailyMap = daily.ToDictionary(d => d.Day);
        var dailyViews = new List<StatPoint>();
        var dailyVisitors = new List<StatPoint>();
        for (var d = rangeStart; d <= today; d = d.AddDays(1))
        {
            var label = d.ToString("dd MMM");
            dailyMap.TryGetValue(d, out var row);
            dailyViews.Add(new StatPoint(label, row?.Views ?? 0));
            dailyVisitors.Add(new StatPoint(label, row?.Visitors ?? 0));
        }

        return new AnalyticsSummary
        {
            ViewsToday = await views.CountAsync(v => v.Day == today),
            ViewsWeek = await views.CountAsync(v => v.Day >= weekAgo),
            ViewsMonth = await views.CountAsync(v => v.Day >= monthAgo),
            ViewsTotal = await views.CountAsync(),

            VisitorsToday = await views.Where(v => v.Day == today).Select(v => v.VisitorId).Distinct().CountAsync(),
            VisitorsWeek = await views.Where(v => v.Day >= weekAgo).Select(v => v.VisitorId).Distinct().CountAsync(),
            VisitorsMonth = await views.Where(v => v.Day >= monthAgo).Select(v => v.VisitorId).Distinct().CountAsync(),
            VisitorsTotal = await views.Select(v => v.VisitorId).Distinct().CountAsync(),

            SubscribersTotal = await _db.Subscribers.CountAsync(s => s.IsActive),
            SubscribersMonth = await _db.Subscribers.CountAsync(s => s.IsActive && s.SubscribedAt >= monthStartUtc),
            MessagesUnread = await _db.ContactMessages.CountAsync(m => !m.IsRead),
            MessagesTotal = await _db.ContactMessages.CountAsync(),

            DailyViews = dailyViews,
            DailyVisitors = dailyVisitors,

            // Group by a plain column (Path) — a null-coalescing group key does not translate to SQL.
            // The friendlier Title is pulled through with Max and applied after materialisation.
            TopPages = (await views.Where(v => v.Day >= monthAgo)
                    .GroupBy(v => v.Path)
                    .Select(g => new { Path = g.Key, Title = g.Max(x => x.Title), Count = g.Count() })
                    .OrderByDescending(x => x.Count).Take(8).ToListAsync())
                .Select(x => new StatPoint(string.IsNullOrWhiteSpace(x.Title) ? x.Path : x.Title!, x.Count))
                .ToList(),

            TopReferrers = (await views.Where(v => v.Day >= monthAgo && v.ReferrerHost != null)
                    .GroupBy(v => v.ReferrerHost!)
                    .Select(g => new { Key = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count).Take(8).ToListAsync())
                .Select(x => new StatPoint(x.Key, x.Count)).ToList(),

            Devices = (await views.Where(v => v.Day >= monthAgo && v.DeviceType != null)
                    .GroupBy(v => v.DeviceType!)
                    .Select(g => new { Key = g.Key, Count = g.Count() })
                    .OrderByDescending(x => x.Count).ToListAsync())
                .Select(x => new StatPoint(x.Key, x.Count)).ToList(),

            TopProjects = (await _db.Projects.AsNoTracking()
                    .Where(p => p.ViewCount > 0)
                    .OrderByDescending(p => p.ViewCount).Take(6)
                    .Select(p => new { p.Title, p.ViewCount }).ToListAsync())
                .Select(x => new StatPoint(x.Title, x.ViewCount)).ToList(),

            TopPosts = (await _db.BlogPosts.AsNoTracking()
                    .Where(p => p.ViewCount > 0)
                    .OrderByDescending(p => p.ViewCount).Take(6)
                    .Select(p => new { p.Title, p.ViewCount }).ToListAsync())
                .Select(x => new StatPoint(x.Title, x.ViewCount)).ToList(),
        };
    }

    private static string DeviceFrom(string ua) =>
        ua.Contains("ipad") || ua.Contains("tablet") ? "tablet"
        : ua.Contains("mobi") || ua.Contains("android") || ua.Contains("iphone") ? "mobile"
        : "desktop";

    /// <summary>Salted one-way hash — we count unique visitors without storing an IP address.</summary>
    private static string? HashIp(string? ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return null;
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes("zp-analytics::" + ip));
        return Convert.ToHexString(bytes)[..32];
    }

    private static string? Truncate(string? value, int max) =>
        string.IsNullOrEmpty(value) ? value : value.Length <= max ? value : value[..max];
}
