namespace WorkLogApp.Models;

public class RecurringEntry
{
    public int      Id            { get; set; }
    public int      UserId        { get; set; }
    public User     User          { get; set; } = null!;
    public string   Project       { get; set; } = "";
    public string   Description   { get; set; } = "";
    public decimal  Hours         { get; set; }
    public string   Tags          { get; set; } = "";
    /* "daily" | "weekdays" | "mon,wed,fri" etc. */
    public string   Schedule      { get; set; } = "weekdays";
    public bool     IsActive      { get; set; } = true;
    public DateOnly? LastFiredDate { get; set; }
    public DateTime  CreatedAt    { get; set; } = DateTime.UtcNow;

    public string ScheduleLabel => Schedule switch
    {
        "daily"    => "Every day",
        "weekdays" => "Every weekday",
        _ => string.Join(", ", Schedule.Split(',').Select(Abbrev))
    };

    public bool ShouldFireToday()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        if (!IsActive || LastFiredDate == today) return false;
        var dow = today.DayOfWeek;
        return Schedule switch
        {
            "daily"    => true,
            "weekdays" => dow is >= DayOfWeek.Monday and <= DayOfWeek.Friday,
            _ => Schedule.Split(',').Select(d => d.Trim().ToLower()).Contains(DowAbbr(dow))
        };
    }

    private static string DowAbbr(DayOfWeek d) => d switch
    {
        DayOfWeek.Monday    => "mon", DayOfWeek.Tuesday  => "tue",
        DayOfWeek.Wednesday => "wed", DayOfWeek.Thursday => "thu",
        DayOfWeek.Friday    => "fri", DayOfWeek.Saturday => "sat",
        _                   => "sun"
    };

    private static string Abbrev(string d) => d.Trim().ToLower() switch
    {
        "mon" => "Mon", "tue" => "Tue", "wed" => "Wed", "thu" => "Thu",
        "fri" => "Fri", "sat" => "Sat", "sun" => "Sun", _ => d.Trim()
    };
}
