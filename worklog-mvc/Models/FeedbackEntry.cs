namespace WorkLogApp.Models;

public class FeedbackEntry
{
    public int      Id        { get; set; }
    public int?     UserId    { get; set; }
    public string   UserName  { get; set; } = "";
    public int      Rating    { get; set; }
    public string   Category  { get; set; } = "General Feedback";
    public string   Message   { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
