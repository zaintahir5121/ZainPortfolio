namespace WorkLogApp.Models;

public class ChecklistItem
{
    public int    Id        { get; set; }
    public int    NoteId    { get; set; }
    public Note   Note      { get; set; } = null!;
    public string Text      { get; set; } = "";
    public bool   IsChecked { get; set; }
    public int    SortOrder { get; set; }
}
