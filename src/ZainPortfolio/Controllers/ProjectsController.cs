using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models.ViewModels;
using ZainPortfolio.Services;

namespace ZainPortfolio.Controllers;

[Route("projects")]
public class ProjectsController : Controller
{
    private const int PageSize = 12;

    private readonly ApplicationDbContext _db;
    private readonly ISiteSettingsService _settings;
    private readonly IAnalyticsService _analytics;

    public ProjectsController(ApplicationDbContext db, ISiteSettingsService settings, IAnalyticsService analytics)
    {
        _db = db;
        _settings = settings;
        _analytics = analytics;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? category, string? q, int page = 1)
    {
        var settings = await _settings.GetAsync();
        if (!settings.EnableProjects) return NotFound();

        var query = _db.Projects.AsNoTracking()
            .Include(p => p.ProjectCategory)
            .Where(p => p.IsPublished);

        if (!string.IsNullOrWhiteSpace(category) && category != "all")
            query = query.Where(p =>
                (p.ProjectCategory != null && p.ProjectCategory.Slug == category) ||
                (p.FilterKeys != null && p.FilterKeys.Contains(category)));

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim();
            query = query.Where(p =>
                p.Title.Contains(term) ||
                (p.Summary != null && p.Summary.Contains(term)) ||
                (p.Tags != null && p.Tags.Contains(term)));
        }

        var total = await query.CountAsync();
        page = Math.Max(1, page);

        var items = await query
            .OrderBy(p => p.SortOrder).ThenByDescending(p => p.CreatedAt)
            .Skip((page - 1) * PageSize).Take(PageSize)
            .ToListAsync();

        await _analytics.TrackAsync(HttpContext, "list", null, "Projects");
        return View(new ProjectListViewModel
        {
            Settings = settings,
            Projects = items,
            Categories = await _db.ProjectCategories.AsNoTracking().OrderBy(c => c.SortOrder).ToListAsync(),
            CurrentCategory = category ?? "all",
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
        if (!settings.EnableProjects) return NotFound();

        var project = await _db.Projects
            .Include(p => p.ProjectCategory)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished);

        if (project is null) return NotFound();

        // Fire-and-forget style view counter — never let it break the page render.
        try
        {
            project.ViewCount++;
            await _db.SaveChangesAsync();
        }
        catch { /* view count is not critical */ }

        var related = await _db.Projects.AsNoTracking()
            .Where(p => p.IsPublished && p.Id != project.Id &&
                        (p.ProjectCategoryId == project.ProjectCategoryId || p.IsFeatured))
            .OrderBy(p => p.SortOrder)
            .Take(3).ToListAsync();

        await _analytics.TrackAsync(HttpContext, "project", project.Id, project.Title);
        return View(new ProjectDetailViewModel
        {
            Settings = settings,
            Project = project,
            Related = related
        });
    }
}
