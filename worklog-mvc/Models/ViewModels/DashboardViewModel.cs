namespace WorkLogApp.Models.ViewModels;

public record WeekDayStat(string Name, decimal Hours, bool IsToday, bool IsFuture)
{
    public string Formatted
    {
        get
        {
            if (Hours == 0) return "—";
            var h = (int)Hours;
            var m = (int)Math.Round((Hours - h) * 60);
            return m == 0 ? $"{h}h" : $"{h}h {m}m";
        }
    }
}

public class DashboardViewModel
{
    public List<LogEntry> Logs        { get; set; } = [];
    public string         Period      { get; set; } = "all";
    public string         SearchQuery { get; set; } = "";
    public string         UserName    { get; set; } = "";
    public bool           IsAdmin     { get; set; }

    /* sidebar / nav stats */
    public decimal TodayHours    { get; set; }
    public int     WeekLogsCount { get; set; }
    public int     ProjectsCount { get; set; }

    /* insights */
    public int     Streak            { get; set; }
    public decimal WeekHours         { get; set; }
    public decimal LastWeekHours     { get; set; }
    public string  TopProject        { get; set; } = "";
    public decimal TopProjectHours   { get; set; }
    public bool    WarnNotLogged     { get; set; }

    /* autocomplete */
    public List<string> RecentProjects { get; set; } = [];

    /* weekly sidebar */
    public List<WeekDayStat> WeekDays   { get; set; } = [];
    public decimal           WeekGoal   { get; set; } = 40m;
    public string WeekTotalFormatted
    {
        get
        {
            var h = (int)WeekHours;
            var m = (int)Math.Round((WeekHours - h) * 60);
            return m == 0 ? $"{h}h" : $"{h}h {m}m";
        }
    }
    public int    WeekPctInt => (int)Math.Min(100, Math.Round(WeekGoal > 0 ? WeekHours / WeekGoal * 100 : 0));
    public double SvgOffset  => 314.159 * (1.0 - Math.Min(1.0, (double)(WeekGoal > 0 ? WeekHours / WeekGoal : 0)));
    public string ProgressMessage =>
        WeekPctInt >= 100 ? "🎉 Goal reached — excellent week!"   :
        WeekPctInt >=  75 ? "Great job! You're on track 🙌"        :
        WeekPctInt >=  50 ? "Halfway there — keep pushing!"         :
        WeekPctInt >=  25 ? "Good start — build that momentum!"     :
                            "Let's get this week going!";

    public string Greeting =>
        DateTime.Now.Hour switch { < 12 => "Good morning", < 17 => "Good afternoon", _ => "Good evening" };

    public string FirstName =>
        string.IsNullOrWhiteSpace(UserName) ? "there"
            : UserName.Split(' ', StringSplitOptions.RemoveEmptyEntries)[0];
}
