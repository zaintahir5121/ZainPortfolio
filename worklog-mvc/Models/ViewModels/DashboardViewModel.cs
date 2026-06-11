namespace WorkLogApp.Models.ViewModels;

public class DashboardViewModel
{
    public List<LogEntry> Logs          { get; set; } = [];
    public string         Period        { get; set; } = "all";
    public string         SearchQuery   { get; set; } = "";

    public string  UserName      { get; set; } = "";
    public bool    IsAdmin       { get; set; }

    public decimal TodayHours    { get; set; }
    public int     WeekLogsCount { get; set; }
    public int     ProjectsCount { get; set; }

    public string Greeting =>
        DateTime.Now.Hour switch
        {
            < 12 => "Good morning",
            < 17 => "Good afternoon",
            _    => "Good evening",
        };
}
