using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models;

namespace WorkLogApp.Controllers;

[Authorize]
public class NotesController(AppDbContext db) : Controller
{
    private int Uid => int.Parse(User.FindFirst("UserId")!.Value);

    // ── Pages ──────────────────────────────────────────────────────────────────

    public async Task<IActionResult> Index()
    {
        var notes = await db.Notes
            .Where(n => n.UserId == Uid && !n.IsArchived && !n.IsDeleted)
            .Include(n => n.ChecklistItems.OrderBy(c => c.SortOrder))
            .OrderByDescending(n => n.IsPinned)
            .ThenByDescending(n => n.UpdatedAt)
            .ToListAsync();
        ViewBag.Section = "notes";
        return View(notes);
    }

    public async Task<IActionResult> Archive()
    {
        var notes = await db.Notes
            .Where(n => n.UserId == Uid && n.IsArchived && !n.IsDeleted)
            .Include(n => n.ChecklistItems.OrderBy(c => c.SortOrder))
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();
        ViewBag.Section = "archive";
        return View("Index", notes);
    }

    public async Task<IActionResult> Trash()
    {
        var notes = await db.Notes
            .Where(n => n.UserId == Uid && n.IsDeleted)
            .Include(n => n.ChecklistItems.OrderBy(c => c.SortOrder))
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();
        ViewBag.Section = "trash";
        return View("Index", notes);
    }

    // ── CRUD API ───────────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReq req)
    {
        if (string.IsNullOrWhiteSpace(req.Title) && string.IsNullOrWhiteSpace(req.Content)
            && (req.Items == null || req.Items.Count == 0))
            return BadRequest();

        var note = new Note
        {
            UserId    = Uid,
            Title     = req.Title?.Trim() ?? "",
            Content   = req.Content?.Trim() ?? "",
            Color     = req.Color ?? "default",
            IsPinned  = req.Pinned,
            NoteType  = req.Items?.Count > 0 ? "list" : "note",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        db.Notes.Add(note);
        await db.SaveChangesAsync();

        if (req.Items?.Count > 0)
        {
            int order = 0;
            foreach (var item in req.Items)
                db.ChecklistItems.Add(new ChecklistItem { NoteId = note.Id, Text = item.Text, IsChecked = item.Checked, SortOrder = order++ });
            await db.SaveChangesAsync();
            await db.Entry(note).Collection(n => n.ChecklistItems).LoadAsync();
        }

        return Ok(ToDto(note));
    }

    [HttpPost]
    public async Task<IActionResult> Update([FromBody] UpdateReq req)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == req.Id && n.UserId == Uid);
        if (note == null) return NotFound();

        if (req.Title    != null) note.Title      = req.Title.Trim();
        if (req.Content  != null) note.Content    = req.Content.Trim();
        if (req.Color    != null) note.Color      = req.Color;
        if (req.Pinned   != null) note.IsPinned   = req.Pinned.Value;
        if (req.Archived != null) note.IsArchived = req.Archived.Value;
        if (req.Deleted  != null) note.IsDeleted  = req.Deleted.Value;
        note.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Delete([FromBody] IdReq req)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == req.Id && n.UserId == Uid);
        if (note == null) return NotFound();
        note.IsDeleted = true; note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> Restore([FromBody] IdReq req)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == req.Id && n.UserId == Uid);
        if (note == null) return NotFound();
        note.IsDeleted = false; note.IsArchived = false; note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteForever([FromBody] IdReq req)
    {
        var note = await db.Notes.Include(n => n.ChecklistItems)
                                  .FirstOrDefaultAsync(n => n.Id == req.Id && n.UserId == Uid);
        if (note == null) return NotFound();
        db.Notes.Remove(note);
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> EmptyTrash()
    {
        var notes = await db.Notes.Where(n => n.UserId == Uid && n.IsDeleted)
                                   .Include(n => n.ChecklistItems).ToListAsync();
        db.Notes.RemoveRange(notes);
        await db.SaveChangesAsync();
        return Ok();
    }

    // ── Checklist items ────────────────────────────────────────────────────────

    [HttpPost]
    public async Task<IActionResult> AddItem([FromBody] AddItemReq req)
    {
        var note = await db.Notes.FirstOrDefaultAsync(n => n.Id == req.NoteId && n.UserId == Uid);
        if (note == null) return NotFound();
        var maxOrder = await db.ChecklistItems.Where(c => c.NoteId == req.NoteId).MaxAsync(c => (int?)c.SortOrder) ?? -1;
        var item = new ChecklistItem { NoteId = req.NoteId, Text = req.Text, SortOrder = maxOrder + 1 };
        db.ChecklistItems.Add(item);
        note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok(new { item.Id, item.Text, item.IsChecked, item.SortOrder });
    }

    [HttpPost]
    public async Task<IActionResult> UpdateItem([FromBody] UpdateItemReq req)
    {
        var item = await db.ChecklistItems.Include(c => c.Note)
                                           .FirstOrDefaultAsync(c => c.Id == req.Id && c.Note.UserId == Uid);
        if (item == null) return NotFound();
        if (req.Text    != null) item.Text      = req.Text;
        if (req.Checked != null) item.IsChecked = req.Checked.Value;
        item.Note.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return Ok();
    }

    [HttpPost]
    public async Task<IActionResult> DeleteItem([FromBody] IdReq req)
    {
        var item = await db.ChecklistItems.Include(c => c.Note)
                                           .FirstOrDefaultAsync(c => c.Id == req.Id && c.Note.UserId == Uid);
        if (item == null) return NotFound();
        item.Note.UpdatedAt = DateTime.UtcNow;
        db.ChecklistItems.Remove(item);
        await db.SaveChangesAsync();
        return Ok();
    }

    // ── Helpers ────────────────────────────────────────────────────────────────

    static object ToDto(Note n) => new
    {
        n.Id, n.Title, n.Content, n.Color, n.IsPinned, n.IsArchived, n.IsDeleted, n.NoteType,
        n.CreatedAt, n.UpdatedAt,
        items = n.ChecklistItems.OrderBy(c => c.SortOrder)
                                .Select(c => new { c.Id, c.Text, c.IsChecked }),
    };

    // ── Request records ────────────────────────────────────────────────────────

    public record CreateReq(string? Title, string? Content, string? Color, bool Pinned, List<ItemInput>? Items);
    public record ItemInput(string Text, bool Checked);
    public record UpdateReq(int Id, string? Title, string? Content, string? Color, bool? Pinned, bool? Archived, bool? Deleted);
    public record IdReq(int Id);
    public record AddItemReq(int NoteId, string Text);
    public record UpdateItemReq(int Id, string? Text, bool? Checked);
}
