using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;
using ZainPortfolio.Services;

namespace ZainPortfolio.Areas.Admin.Controllers;

// ------------------------------------------------------------------ BLOG
[Area("Admin")]
[Authorize]
[Route("admin/blog")]
public class BlogController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHtmlSanitizerService _sanitizer;

    public BlogController(ApplicationDbContext db, IHtmlSanitizerService sanitizer)
    {
        _db = db;
        _sanitizer = sanitizer;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.BlogPosts.AsNoTracking().Include(p => p.BlogCategory).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Title.Contains(q) || (p.Tags != null && p.Tags.Contains(q)));
        ViewBag.Query = q;
        return View(await query.OrderByDescending(p => p.PublishedAt).ToListAsync());
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await LoadListsAsync();
        return View("Edit", new BlogPost { IsPublished = true, PublishedAt = DateTime.UtcNow });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _db.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null) return NotFound();
        await LoadListsAsync();
        return View(post);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(BlogPost model)
    {
        ModelState.Remove(nameof(BlogPost.Slug));
        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View("Edit", model);
        }

        var isNew = model.Id == 0;
        var entity = isNew ? new BlogPost() : await _db.BlogPosts.FirstOrDefaultAsync(p => p.Id == model.Id);
        if (entity is null) return NotFound();

        var slugSource = string.IsNullOrWhiteSpace(model.Slug) ? model.Title : model.Slug;
        entity.Slug = await SlugHelper.UniqueAsync(slugSource,
            s => _db.BlogPosts.AnyAsync(p => p.Slug == s && p.Id != model.Id),
            isNew ? null : entity.Slug);

        entity.Title = model.Title;
        entity.Content = _sanitizer.Sanitize(model.Content);
        entity.Excerpt = string.IsNullOrWhiteSpace(model.Excerpt)
            ? TextHelpers.Plain(entity.Content, 220)
            : model.Excerpt;
        entity.CoverImageUrl = model.CoverImageUrl;
        entity.VideoId = ProjectsController.ExtractVideoId(model.VideoId);
        entity.AuthorName = string.IsNullOrWhiteSpace(model.AuthorName) ? "Zain Abbas Tahir" : model.AuthorName;
        entity.AuthorImageUrl = model.AuthorImageUrl;
        entity.ReadMinutes = model.ReadMinutes > 0 ? model.ReadMinutes : TextHelpers.EstimateReadMinutes(entity.Content);
        entity.Tags = model.Tags;
        entity.BlogCategoryId = model.BlogCategoryId;
        entity.IsPublished = model.IsPublished;
        entity.IsFeatured = model.IsFeatured;
        entity.PublishedAt = model.PublishedAt == default ? DateTime.UtcNow : model.PublishedAt;
        entity.MetaTitle = model.MetaTitle;
        entity.MetaDescription = model.MetaDescription;
        entity.UpdatedAt = DateTime.UtcNow;

        if (isNew)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _db.BlogPosts.Add(entity);
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = isNew ? "Post created." : "Post updated.";
        return RedirectToAction(nameof(Edit), new { id = entity.Id });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post is not null)
        {
            _db.BlogPosts.Remove(post);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Post deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var post = await _db.BlogPosts.FindAsync(id);
        if (post is not null)
        {
            post.IsPublished = !post.IsPublished;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadListsAsync() =>
        ViewBag.Categories = new SelectList(
            await _db.BlogCategories.OrderBy(c => c.SortOrder).ToListAsync(), "Id", "Name");
}

// ------------------------------------------------------------------ MEDIA
[Area("Admin")]
[Authorize]
[Route("admin/media")]
public class MediaController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IMediaService _media;

    public MediaController(ApplicationDbContext db, IMediaService media)
    {
        _db = db;
        _media = media;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? kind)
    {
        var query = _db.MediaAssets.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(kind) && kind != "all")
            query = query.Where(m => m.Kind == kind);

        ViewBag.Kind = kind ?? "all";
        ViewBag.TotalSize = await _db.MediaAssets.SumAsync(m => (long?)m.SizeBytes) ?? 0;
        return View(await query.OrderByDescending(m => m.UploadedAt).ToListAsync());
    }

    [HttpPost("upload")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(64L * 1024 * 1024)]
    public async Task<IActionResult> Upload(List<IFormFile> files, string? altText)
    {
        if (files is null || files.Count == 0)
        {
            TempData["Error"] = "Choose at least one file to upload.";
            return RedirectToAction(nameof(Index));
        }

        var ok = 0;
        var errors = new List<string>();
        foreach (var file in files)
        {
            var (success, error, _) = await _media.SaveAsync(file, altText);
            if (success) ok++;
            else errors.Add($"{file.FileName}: {error}");
        }

        if (ok > 0) TempData["Success"] = $"{ok} file(s) uploaded.";
        if (errors.Count > 0) TempData["Error"] = string.Join(" | ", errors);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Used by the in-page media picker (returns JSON for the modal).</summary>
    [HttpPost("upload-ajax")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAjax(IFormFile file)
    {
        var (ok, error, asset) = await _media.SaveAsync(file);
        return ok
            ? Json(new { ok = true, url = asset!.Url, kind = asset.Kind, name = asset.OriginalName })
            : Json(new { ok = false, message = error });
    }

    [HttpGet("picker")]
    public async Task<IActionResult> Picker(string? kind)
    {
        var query = _db.MediaAssets.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(kind) && kind != "all")
            query = query.Where(m => m.Kind == kind);
        return Json(await query.OrderByDescending(m => m.UploadedAt).Take(200)
            .Select(m => new { m.Id, m.Url, m.Kind, m.OriginalName }).ToListAsync());
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _media.DeleteAsync(id);
        TempData[deleted ? "Success" : "Error"] = deleted ? "File deleted." : "File not found.";
        return RedirectToAction(nameof(Index));
    }
}

// ------------------------------------------------------------------ SETTINGS
[Area("Admin")]
[Authorize]
[Route("admin/settings")]
public class SettingsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ISiteSettingsService _settings;

    public SettingsController(ApplicationDbContext db, ISiteSettingsService settings)
    {
        _db = db;
        _settings = settings;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string tab = "brand")
    {
        ViewBag.Tab = tab;
        var settings = await _db.SiteSettings.FirstOrDefaultAsync();
        if (settings is null)
        {
            settings = new SiteSetting();
            _db.SiteSettings.Add(settings);
            await _db.SaveChangesAsync();
            _settings.Invalidate();
        }
        return View(settings);
    }

    [HttpPost("")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(SiteSetting model, string tab = "brand")
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Tab = tab;
            return View(model);
        }

        var entity = await _db.SiteSettings.FirstOrDefaultAsync();
        if (entity is null)
        {
            model.Id = 0;
            _db.SiteSettings.Add(model);
        }
        else
        {
            model.Id = entity.Id;
            model.UpdatedAt = DateTime.UtcNow;
            _db.Entry(entity).CurrentValues.SetValues(model);
        }

        await _db.SaveChangesAsync();
        _settings.Invalidate();     // drop the cached copy so the public site picks it up immediately

        TempData["Success"] = "Settings saved. Refresh the public site to see the changes.";
        return RedirectToAction(nameof(Index), new { tab });
    }
}

// ------------------------------------------------------------------ TAXONOMY, SKILLS, EXPERIENCE
[Area("Admin")]
[Authorize]
[Route("admin/structure")]
public class StructureController : Controller
{
    private readonly ApplicationDbContext _db;
    public StructureController(ApplicationDbContext db) => _db = db;

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        ViewBag.ProjectCategories = await _db.ProjectCategories.OrderBy(c => c.SortOrder).ToListAsync();
        ViewBag.BlogCategories = await _db.BlogCategories.OrderBy(c => c.SortOrder).ToListAsync();
        ViewBag.SkillCategories = await _db.SkillCategories
            .Include(c => c.Skills.OrderBy(s => s.SortOrder))
            .OrderBy(c => c.SortOrder).ToListAsync();
        ViewBag.Experiences = await _db.Experiences.OrderBy(e => e.SortOrder).ToListAsync();
        return View();
    }

    [HttpPost("project-category")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveProjectCategory(int id, string name, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToAction(nameof(Index));
        if (id == 0)
            _db.ProjectCategories.Add(new ProjectCategory
            {
                Name = name, Slug = SlugHelper.Generate(name), SortOrder = sortOrder
            });
        else
        {
            var c = await _db.ProjectCategories.FindAsync(id);
            if (c is not null) { c.Name = name; c.SortOrder = sortOrder; }
        }
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("project-category/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProjectCategory(int id)
    {
        var c = await _db.ProjectCategories.FindAsync(id);
        if (c is not null) { _db.ProjectCategories.Remove(c); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("blog-category")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveBlogCategory(int id, string name, string? description, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToAction(nameof(Index));
        if (id == 0)
            _db.BlogCategories.Add(new BlogCategory
            {
                Name = name, Slug = SlugHelper.Generate(name), Description = description, SortOrder = sortOrder
            });
        else
        {
            var c = await _db.BlogCategories.FindAsync(id);
            if (c is not null) { c.Name = name; c.Description = description; c.SortOrder = sortOrder; }
        }
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("blog-category/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBlogCategory(int id)
    {
        var c = await _db.BlogCategories.FindAsync(id);
        if (c is not null) { _db.BlogCategories.Remove(c); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("skill-category")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSkillCategory(int id, string name, string iconClass, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToAction(nameof(Index));
        if (id == 0)
            _db.SkillCategories.Add(new SkillCategory { Name = name, IconClass = iconClass, SortOrder = sortOrder });
        else
        {
            var c = await _db.SkillCategories.FindAsync(id);
            if (c is not null) { c.Name = name; c.IconClass = iconClass; c.SortOrder = sortOrder; }
        }
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("skill-category/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkillCategory(int id)
    {
        var c = await _db.SkillCategories.FindAsync(id);
        if (c is not null) { _db.SkillCategories.Remove(c); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("skill")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveSkill(int id, int skillCategoryId, string name, int percentage, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name)) return RedirectToAction(nameof(Index));
        percentage = Math.Clamp(percentage, 0, 100);
        if (id == 0)
            _db.Skills.Add(new Skill
            {
                SkillCategoryId = skillCategoryId, Name = name, Percentage = percentage, SortOrder = sortOrder
            });
        else
        {
            var s = await _db.Skills.FindAsync(id);
            if (s is not null) { s.Name = name; s.Percentage = percentage; s.SortOrder = sortOrder; }
        }
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("skill/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSkill(int id)
    {
        var s = await _db.Skills.FindAsync(id);
        if (s is not null) { _db.Skills.Remove(s); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("experience/{id:int}")]
    public async Task<IActionResult> EditExperience(int id)
    {
        var e = id == 0 ? new Experience() : await _db.Experiences.FindAsync(id);
        if (e is null) return NotFound();
        return View(e);
    }

    [HttpPost("experience")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveExperience(Experience model)
    {
        if (!ModelState.IsValid) return View("EditExperience", model);

        if (model.Id == 0) _db.Experiences.Add(model);
        else
        {
            var e = await _db.Experiences.FindAsync(model.Id);
            if (e is null) return NotFound();
            e.Role = model.Role; e.Company = model.Company; e.Location = model.Location;
            e.DateRange = model.DateRange; e.IsCurrent = model.IsCurrent;
            e.Bullets = model.Bullets; e.Tags = model.Tags; e.SortOrder = model.SortOrder;
        }
        await _db.SaveChangesAsync();
        TempData["Success"] = "Experience saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("experience/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteExperience(int id)
    {
        var e = await _db.Experiences.FindAsync(id);
        if (e is not null) { _db.Experiences.Remove(e); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
