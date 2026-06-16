namespace WorkLogApp.Models;

public class User
{
    public int    Id           { get; set; }
    public string Username     { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Name         { get; set; } = "";
    public string Role         { get; set; } = "employee";   // "admin" | "employee"

    public string? ApiKey { get; set; }

    public ICollection<Note> Notes { get; set; } = [];
}
