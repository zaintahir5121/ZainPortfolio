using System.ComponentModel.DataAnnotations;

namespace WorkLogApp.Models.ViewModels;

public class AddLogViewModel
{
    [Required] public DateOnly Date        { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    [Required] public string   Project     { get; set; } = "";
    [Required] public string   Description { get; set; } = "";

    [Required, Range(0.5, 24)]
    public decimal Hours { get; set; }

    public string Tags { get; set; } = "";
}
