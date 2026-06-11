namespace WorkLogApp.Models;

public class User
{
    public int    Id           { get; set; }
    public string Username     { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Name         { get; set; } = "";
    public string Role         { get; set; } = "employee";   // "admin" | "employee"

    public ICollection<LogEntry> LogEntries { get; set; } = [];
}
