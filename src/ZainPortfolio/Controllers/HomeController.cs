using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;
using ZainPortfolio.Models.ViewModels;
using ZainPortfolio.Services;

namespace ZainPortfolio.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ISiteSettingsService _settings;
    private readonly IAnalyticsService _analytics;

    public HomeController(ApplicationDbContext db, ISiteSettingsService settings, IAnalyticsService analytics)
    {
        _db = db;
        _settings = settings;
        _analytics = analytics;
    }

    public async Task<IActionResult> Index()
    {
        var settings = await _settings.GetAsync();

        var vm = new HomeViewModel
        {
            Settings = settings,
            FeaturedProjects = await _db.Projects.AsNoTracking()
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderBy(p => p.SortOrder)
                .Take(6)
                .ToListAsync(),
            ProjectCategories = await _db.ProjectCategories.AsNoTracking()
                .OrderBy(c => c.SortOrder).ToListAsync(),
            SkillCategories = await _db.SkillCategories.AsNoTracking()
                .Include(c => c.Skills.OrderBy(s => s.SortOrder))
                .OrderBy(c => c.SortOrder).ToListAsync(),
            Experiences = await _db.Experiences.AsNoTracking()
                .OrderBy(e => e.SortOrder).ToListAsync(),
            LatestPosts = settings.EnableBlog
                ? await _db.BlogPosts.AsNoTracking()
                    .Include(p => p.BlogCategory)
                    .Where(p => p.IsPublished)
                    .OrderByDescending(p => p.PublishedAt)
                    .Take(3).ToListAsync()
                : new List<BlogPost>(),
            TotalProjects = await _db.Projects.CountAsync(p => p.IsPublished)
        };

        await _analytics.TrackAsync(HttpContext, "home", null, "Home");
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Contact(ContactMessage model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ContactError"] = "Please complete all required fields with a valid email address.";
            return RedirectToAction(nameof(Index), null, "contact");
        }

        model.CreatedAt = DateTime.UtcNow;
        model.IsRead = false;
        _db.ContactMessages.Add(model);
        await _db.SaveChangesAsync();

        TempData["ContactSuccess"] = "Thank you — your message has been received. I'll get back to you shortly.";
        return RedirectToAction(nameof(Index), null, "contact");
    }

    [Route("/error/{code:int?}")]
    public async Task<IActionResult> Error(int? code)
    {
        ViewBag.StatusCode = code ?? 500;
        ViewBag.Settings = await _settings.GetAsync();
        return View();
    }
}
