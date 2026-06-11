using System.ComponentModel.DataAnnotations;

namespace WorkLogApp.Models.ViewModels;

public class LoginViewModel
{
    [Required] public string Username { get; set; } = "";
    [Required] public string Password { get; set; } = "";
}
