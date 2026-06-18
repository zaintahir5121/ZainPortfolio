namespace WorkLogApp.Models;

public class Note
{
    public int    Id         { get; set; }
    public int    UserId     { get; set; }
    public User   User       { get; set; } = null!;

    public string Title      { get; set; } = "";
    public string Content    { get; set; } = "";
    public string Color      { get; set; } = "default";
    public bool   IsPinned   { get; set; }
    public bool   IsArchived { get; set; }
    public bool   IsDeleted  { get; set; }
    public string NoteType   { get; set; } = "note"; // "note" | "list"

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ChecklistItem> ChecklistItems { get; set; } = new();

    public string RelativeTime()
    {
        var diff = DateTime.UtcNow - UpdatedAt;
        return diff.TotalDays  > 365 ? UpdatedAt.ToString("MMM d, yyyy")
             : diff.TotalDays  > 30  ? UpdatedAt.ToString("MMM d")
             : diff.TotalDays  > 1   ? $"{(int)diff.TotalDays}d ago"
             : diff.TotalHours > 1   ? $"{(int)diff.TotalHours}h ago"
             : diff.TotalMinutes > 1 ? $"{(int)diff.TotalMinutes}m ago"
             : "just now";
    }
}
