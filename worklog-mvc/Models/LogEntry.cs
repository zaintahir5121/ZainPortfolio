namespace WorkLogApp.Models;

public class LogEntry
{
    public int      Id          { get; set; }
    public int      UserId      { get; set; }
    public User     User        { get; set; } = null!;

    public DateOnly Date        { get; set; }
    public string   Project     { get; set; } = "";
    public string   Description { get; set; } = "";
    public decimal  Hours       { get; set; }
    public string   Tags        { get; set; } = "";   // comma-separated

    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

    public string[] TagList =>
        string.IsNullOrWhiteSpace(Tags)
            ? []
            : Tags.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    public int ColorIndex => Math.Abs(Id % 7);
}
