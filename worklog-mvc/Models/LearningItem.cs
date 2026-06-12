namespace WorkLogApp.Models;

public class LearningItem
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string   Title         { get; set; } = "";
    public string   Type          { get; set; } = "course";      // course | book | article | video | podcast | tutorial | other
    public string   Source        { get; set; } = "";
    public string   Status        { get; set; } = "in-progress"; // want | in-progress | completed
    public string   Notes         { get; set; } = "";
    public DateOnly? StartedDate  { get; set; }
    public DateOnly? CompletedDate { get; set; }
    public int      Rating        { get; set; }                  // 0–5
    public DateTime CreatedAt     { get; set; } = DateTime.UtcNow;

    public string TypeEmoji => Type switch {
        "book"     => "📖",
        "article"  => "📄",
        "video"    => "🎬",
        "podcast"  => "🎙️",
        "tutorial" => "💻",
        "other"    => "💡",
        _          => "🎓",
    };

    public string StatusLabel => Status switch {
        "want"      => "Want to learn",
        "completed" => "Completed",
        _           => "In progress",
    };
}
