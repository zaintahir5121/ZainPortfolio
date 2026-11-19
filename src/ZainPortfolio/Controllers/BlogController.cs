using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;
using ZainPortfolio.Models.ViewModels;
using ZainPortfolio.Services;

namespace ZainPortfolio.Controllers;

[Route("blog")]
public class BlogController : Controller
{
    private const int PageSize = 9;

    private readonly ApplicationDbContext _db;
    private readonly ISiteSettingsService _settings;
    private readonly IAnalyticsService _analytics;

    public BlogController(ApplicationDbContext db, ISiteSettingsService settings, IAnalyticsService analytics)
    {
        _db = db;
        _settings = settings;
        _analytics = analytics;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? category, string? tag, string? q, int page = 1)
    {
        var settings = await _settings.GetAsync();
        if (!settings.EnableBlog) return NotFound();

        var query = _db.BlogPosts.AsNoTracking()
            .Include(p => p.BlogCategory)
            .Where(p => p.IsPublished && p.PublishedAt <= DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.BlogCategory != null && p.BlogCategory.Slug == category);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(p => p.Tags != null && p.Tags.Contains(tag));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p =>
                p.Title.Contains(term) ||
                (p.Excerpt != null && p.Excerpt.Contains(term)) ||
                (p.Tags != null && p.Tags.Contains(term)));
        }

        var total = await query.CountAsync();
        page = Math.Max(1, page);

        var posts = await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * PageSize).Take(PageSize)
            .ToListAsync();

        // Featured hero post only on the unfiltered first page.
        BlogPost? featured = null;
        if (page == 1 && string.IsNullOrWhiteSpace(category) && string.IsNullOrWhiteSpace(tag) && string.IsNullOrWhiteSpace(q))
        {
            featured = await _db.BlogPosts.AsNoTracking()
                .Include(p => p.BlogCategory)
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderByDescending(p => p.PublishedAt)
                .FirstOrDefaultAsync();
            if (featured is not null)
                posts = posts.Where(p => p.Id != featured.Id).ToList();
        }

        var allTags = await _db.BlogPosts.AsNoTracking()
            .Where(p => p.IsPublished && p.Tags != null)
            .Select(p => p.Tags!).ToListAsync();

        var popularTags = allTags
            .SelectMany(t => TextHelpers.Csv(t))
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key).Take(14).ToList();

        await _analytics.TrackAsync(HttpContext, "list", null, "Blog");

        return View(new BlogListViewModel
        {
            Settings = settings,
            Posts = posts,
            Featured = featured,
            Categories = await _db.BlogCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync(),
            PopularTags = popularTags,
            CurrentCategory = category,
            CurrentTag = tag,
            Query = q,
            Page = page,
            TotalPages = (int)Math.Ceiling(total / (double)PageSize),
            TotalCount = total
        });
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> Details(string slug)
    {
        var settings = await _settings.GetAsync();
        if (!settings.EnableBlog) return NotFound();

        var post = await _db.BlogPosts
            .Include(p => p.BlogCategory)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (post is null) return NotFound();

        try
        {
            post.ViewCount++;
            await _db.SaveChangesAsync();
        }
        catch { /* view count is not critical */ }

        var related = await _db.BlogPosts.AsNoTracking()
            .Include(p => p.BlogCategory)
            .Where(p => p.IsPublished && p.Id != post.Id &&
                        (p.BlogCategoryId == post.BlogCategoryId || p.IsFeatured))
            .OrderByDescending(p => p.PublishedAt).Take(3).ToListAsync();

        var previous = await _db.BlogPosts.AsNoTracking()
            .Where(p => p.IsPublished && p.PublishedAt < post.PublishedAt)
            .OrderByDescending(p => p.PublishedAt).FirstOrDefaultAsync();

        var next = await _db.BlogPosts.AsNoTracking()
            .Where(p => p.IsPublished && p.PublishedAt > post.PublishedAt)
            .OrderBy(p => p.PublishedAt).FirstOrDefaultAsync();

        await _analytics.TrackAsync(HttpContext, "blog", post.Id, post.Title);

        return View(new BlogDetailViewModel
        {
            Settings = settings,
            Post = post,
            Related = related,
            Previous = previous,
            Next = next
        });
    }
}
