namespace WorkLogApp.Models.ViewModels;

public class TimesheetViewModel
{
    public DateOnly PeriodStart  { get; set; }
    public DateOnly PeriodEnd    { get; set; }
    public string   PeriodType   { get; set; } = "mid";   // mid | end | full
    public int      Month        { get; set; } = DateTime.Today.Month;
    public int      Year         { get; set; } = DateTime.Today.Year;

    public string   EmployeeName { get; set; } = "";
    public string   Role         { get; set; } = "";

    public List<LogEntry> Entries { get; set; } = [];

    /* ── Computed ── */
    public IEnumerable<IGrouping<DateOnly, LogEntry>> ByDay =>
        Entries.GroupBy(e => e.Date).OrderBy(g => g.Key);

    public IEnumerable<IGrouping<string, LogEntry>> ByProject =>
        Entries.GroupBy(e => e.Project)
               .OrderByDescending(g => g.Sum(e => e.Hours));

    public decimal TotalHours    => Entries.Sum(e => e.Hours);
    public int     DaysWorked    => Entries.Select(e => e.Date).Distinct().Count();
    public int     ProjectCount  => Entries.Select(e => e.Project).Distinct().Count();

    public string  PeriodLabel   => $"{PeriodStart:MMMM d} – {PeriodEnd:MMMM d, yyyy}";
    public string? AiSummary     { get; set; }
}
