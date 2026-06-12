namespace WorkLogApp.Models;

public class Achievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title       { get; set; } = "";
    public string Description { get; set; } = "";
    public DateOnly Date      { get; set; }
    public string Category    { get; set; } = "milestone"; // milestone | cert | award | work | personal
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string CategoryEmoji => Category switch {
        "cert"     => "🎓",
        "award"    => "🏅",
        "work"     => "💼",
        "personal" => "⭐",
        _          => "🏆",
    };

    public string CategoryLabel => Category switch {
        "cert"     => "Certification",
        "award"    => "Award",
        "work"     => "Work win",
        "personal" => "Personal",
        _          => "Milestone",
    };
}
