using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;
using ZainPortfolio.Services;

namespace ZainPortfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize]
[Route("admin/projects")]
public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IHtmlSanitizerService _sanitizer;

    public ProjectsController(ApplicationDbContext db, IHtmlSanitizerService sanitizer)
    {
        _db = db;
        _sanitizer = sanitizer;
    }

    [HttpGet("")]
    public async Task<IActionResult> Index(string? q)
    {
        var query = _db.Projects.AsNoTracking().Include(p => p.ProjectCategory).AsQueryable();
        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(p => p.Title.Contains(q) || (p.Tags != null && p.Tags.Contains(q)));

        ViewBag.Query = q;
        return View(await query.OrderBy(p => p.SortOrder).ThenBy(p => p.Title).ToListAsync());
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await LoadListsAsync();
        var maxOrder = await _db.Projects.AnyAsync() ? await _db.Projects.MaxAsync(p => p.SortOrder) : 0;
        return View("Edit", new Project { SortOrder = maxOrder + 1, IsPublished = true });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var project = await _db.Projects.Include(p => p.Images).FirstOrDefaultAsync(p => p.Id == id);
        if (project is null) return NotFound();
        await LoadListsAsync();
        return View(project);
    }

    [HttpPost("save")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(Project model)
    {
        ModelState.Remove(nameof(Project.Slug));
        if (!ModelState.IsValid)
        {
            await LoadListsAsync();
            return View("Edit", model);
        }

        var isNew = model.Id == 0;
        var entity = isNew ? new Project() : await _db.Projects.FirstOrDefaultAsync(p => p.Id == model.Id);
        if (entity is null) return NotFound();

        var slugSource = string.IsNullOrWhiteSpace(model.Slug) ? model.Title : model.Slug;
        entity.Slug = await SlugHelper.UniqueAsync(slugSource,
            s => _db.Projects.AnyAsync(p => p.Slug == s && p.Id != model.Id),
            isNew ? null : entity.Slug);

        entity.Title = model.Title;
        entity.Summary = model.Summary;
        entity.Problem = model.Problem;
        entity.Solution = model.Solution;
        entity.Content = _sanitizer.Sanitize(model.Content);
        entity.CoverImageUrl = model.CoverImageUrl;
        entity.IconClass = string.IsNullOrWhiteSpace(model.IconClass) ? "fas fa-cube" : model.IconClass;
        entity.GradientFrom = model.GradientFrom;
        entity.GradientTo = model.GradientTo;
        entity.VideoId = ExtractVideoId(model.VideoId);
        entity.LiveUrl = model.LiveUrl;
        entity.RepoUrl = model.RepoUrl;
        entity.Tags = model.Tags;
        entity.Stat1Icon = model.Stat1Icon;
        entity.Stat1Text = model.Stat1Text;
        entity.Stat2Icon = model.Stat2Icon;
        entity.Stat2Text = model.Stat2Text;
        entity.ProjectCategoryId = model.ProjectCategoryId;
        entity.FilterKeys = model.FilterKeys;
        entity.IsFeatured = model.IsFeatured;
        entity.IsPublished = model.IsPublished;
        entity.SortOrder = model.SortOrder;
        entity.UpdatedAt = DateTime.UtcNow;

        if (isNew)
        {
            entity.CreatedAt = DateTime.UtcNow;
            _db.Projects.Add(entity);
        }

        await _db.SaveChangesAsync();
        TempData["Success"] = isNew ? "Project created." : "Project updated.";
        return RedirectToAction(nameof(Edit), new { id = entity.Id });
    }

    [HttpPost("{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is not null)
        {
            _db.Projects.Remove(project);
            await _db.SaveChangesAsync();
            TempData["Success"] = "Project deleted.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/publish")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var project = await _db.Projects.FindAsync(id);
        if (project is not null)
        {
            project.IsPublished = !project.IsPublished;
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("{id:int}/images/add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddImage(int id, string url, string? caption)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            var max = await _db.ProjectImages.Where(i => i.ProjectId == id)
                .Select(i => (int?)i.SortOrder).MaxAsync() ?? 0;
            _db.ProjectImages.Add(new ProjectImage
            {
                ProjectId = id, Url = url, Caption = caption, SortOrder = max + 1
            });
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Edit), new { id });
    }

    [HttpPost("images/{imageId:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var img = await _db.ProjectImages.FindAsync(imageId);
        if (img is null) return NotFound();
        var projectId = img.ProjectId;
        _db.ProjectImages.Remove(img);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Edit), new { id = projectId });
    }

    private async Task LoadListsAsync()
    {
        ViewBag.Categories = new SelectList(
            await _db.ProjectCategories.OrderBy(c => c.SortOrder).ToListAsync(), "Id", "Name");
    }

    /// <summary>Accepts a full YouTube URL or a bare id and always stores the id.</summary>
    internal static string? ExtractVideoId(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        input = input.Trim();
        if (!input.Contains('/') && !input.Contains('?')) return input;

        var patterns = new[] { "shorts/", "embed/", "v=", "youtu.be/" };
        foreach (var p in patterns)
        {
            var i = input.IndexOf(p, StringComparison.OrdinalIgnoreCase);
            if (i < 0) continue;
            var id = input[(i + p.Length)..];
            var cut = id.IndexOfAny(new[] { '?', '&', '/', '#' });
            return cut > 0 ? id[..cut] : id;
        }
        return input;
    }
}
