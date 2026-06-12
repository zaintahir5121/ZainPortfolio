namespace WorkLogApp.Models;

public class Experience
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string   Company     { get; set; } = "";
    public string   Role        { get; set; } = "";
    public string   Location    { get; set; } = "";
    public DateOnly StartDate   { get; set; }
    public DateOnly? EndDate    { get; set; }
    public bool     IsCurrent   { get; set; }
    public string   Description { get; set; } = "";
    public string   Tags        { get; set; } = "";
    public DateTime CreatedAt   { get; set; } = DateTime.UtcNow;

    public string Duration
    {
        get
        {
            var end    = IsCurrent ? DateOnly.FromDateTime(DateTime.Today) : (EndDate ?? DateOnly.FromDateTime(DateTime.Today));
            var months = (end.Year - StartDate.Year) * 12 + end.Month - StartDate.Month;
            if (months < 1)  return "< 1 mo";
            if (months < 12) return $"{months} mo";
            var y = months / 12; var m = months % 12;
            return m == 0 ? $"{y} yr" : $"{y} yr {m} mo";
        }
    }
}
