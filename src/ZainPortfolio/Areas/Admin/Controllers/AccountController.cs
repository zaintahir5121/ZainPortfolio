using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Services;

namespace ZainPortfolio.Areas.Admin.Controllers;

[Area("Admin")]
[Route("admin/account")]
public class AccountController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<AccountController> _logger;

    public AccountController(ApplicationDbContext db, ILogger<AccountController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ModelState.AddModelError("", "Enter both a username and a password.");
            return View();
        }

        var user = await _db.AdminUsers.FirstOrDefaultAsync(u => u.Username == username.Trim());

        // Always run the verify path so a missing user and a wrong password take similar time.
        var valid = user is not null && PasswordHasher.Verify(password, user.PasswordHash, user.PasswordSalt);
        if (!valid)
        {
            _logger.LogWarning("Failed admin sign-in for {User}", username);
            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        user!.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("DisplayName", user.DisplayName),
            new(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });

        if (user.MustChangePassword)
        {
            TempData["Warning"] = "You are using the generated password. Please change it now.";
            return RedirectToAction(nameof(ChangePassword));
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("password")]
    [Authorize]
    public IActionResult ChangePassword() => View();

    [HttpPost("password")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
    {
        var user = await _db.AdminUsers.FirstOrDefaultAsync(u => u.Username == User.Identity!.Name);
        if (user is null) return RedirectToAction(nameof(Login));

        if (!PasswordHasher.Verify(currentPassword ?? "", user.PasswordHash, user.PasswordSalt))
            ModelState.AddModelError("", "Your current password is incorrect.");

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 10)
            ModelState.AddModelError("", "The new password must be at least 10 characters.");

        if (newPassword != confirmPassword)
            ModelState.AddModelError("", "The new passwords do not match.");

        if (!ModelState.IsValid) return View();

        var (hash, salt) = PasswordHasher.Hash(newPassword!);
        user.PasswordHash = hash;
        user.PasswordSalt = salt;
        user.MustChangePassword = false;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your password has been changed.";
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }
}
