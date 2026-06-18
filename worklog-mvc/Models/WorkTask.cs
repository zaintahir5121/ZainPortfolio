namespace WorkLogApp.Models;

public class WorkTask
{
    public int       Id          { get; set; }
    public int       WorkEntryId { get; set; }
    public WorkEntry WorkEntry   { get; set; } = null!;
    public string    Description { get; set; } = "";
    public float     Hours       { get; set; }
    public string?   Project     { get; set; }
    public string    Category    { get; set; } = "general"; // development|meeting|review|planning|ops|general
    public int       SortOrder   { get; set; }
}
