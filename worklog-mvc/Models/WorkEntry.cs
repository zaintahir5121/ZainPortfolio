namespace WorkLogApp.Models;

public class WorkEntry
{
    public int      Id         { get; set; }
    public int      UserId     { get; set; }
    public User     User       { get; set; } = null!;
    public string   RawText    { get; set; } = "";
    public DateTime LogDate    { get; set; } = DateTime.UtcNow.Date;
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public float    TotalHours { get; set; }
    public bool     IsAiParsed { get; set; }
    public List<WorkTask> Tasks { get; set; } = [];

    public string RelativeDate()
    {
        var today = DateTime.UtcNow.Date;
        var d = LogDate.Date;
        if (d == today)             return "Today";
        if (d == today.AddDays(-1)) return "Yesterday";
        if (d >= today.AddDays(-6)) return d.ToString("dddd");
        return d.ToString("MMM d");
    }
}
