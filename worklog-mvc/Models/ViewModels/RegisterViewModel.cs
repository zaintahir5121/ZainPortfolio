using System.ComponentModel.DataAnnotations;

namespace WorkLogApp.Models.ViewModels;

public class RegisterViewModel
{
    [Required, MaxLength(80)]
    public string Name { get; set; } = "";

    [Required, MaxLength(40), RegularExpression(@"^[a-zA-Z0-9_\-\.]+$",
        ErrorMessage = "Username can only contain letters, numbers, _ - .")]
    public string Username { get; set; } = "";

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; } = "";
}
