using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Models.ViewModels;

namespace WorkLogApp.Controllers;

public class AccountController(AppDbContext db) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Notes");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user is null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new(ClaimTypes.Name,           user.Name),
            new("username",                user.Username),
            new(ClaimTypes.Role,           user.Role),
        };

        await HttpContext.SignInAsync(
            "Cookies",
            new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")),
            new AuthenticationProperties { IsPersistent = true });

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Notes");
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Notes");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var taken = await db.Users.AnyAsync(u => u.Username == model.Username);
        if (taken)
        {
            ModelState.AddModelError("Username", "That username is already taken — try another.");
            return View(model);
        }

        var user = new WorkLogApp.Models.User
        {
            Name         = model.Name.Trim(),
            Username     = model.Username.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            Role         = "employee",
        };
        db.Users.Add(user);
        await db.SaveChangesAsync();

        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new(ClaimTypes.Name,           user.Name),
            new("username",                user.Username),
            new(ClaimTypes.Role,           user.Role),
        };

        await HttpContext.SignInAsync(
            "Cookies",
            new ClaimsPrincipal(new ClaimsIdentity(claims, "Cookies")),
            new AuthenticationProperties { IsPersistent = true });

        return RedirectToAction("Index", "Notes");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("Cookies");
        return RedirectToAction("Index", "Home");
    }

    /* ── API key for Teams / webhook integration ── */
    [HttpGet]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> ApiKey()
    {
        var uid  = int.Parse(User.FindFirstValue("UserId")!);
        var user = await db.Users.FindAsync(uid);
        if (user is null) return NotFound();

        if (string.IsNullOrWhiteSpace(user.ApiKey))
        {
            user.ApiKey = Guid.NewGuid().ToString("N")[..24];
            await db.SaveChangesAsync();
        }
        return Ok(new { apiKey = user.ApiKey, user = user.Name });
    }

    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> RegenerateKey()
    {
        var uid  = int.Parse(User.FindFirstValue("UserId")!);
        var user = await db.Users.FindAsync(uid);
        if (user is null) return NotFound();
        user.ApiKey = Guid.NewGuid().ToString("N")[..24];
        await db.SaveChangesAsync();
        return Ok(new { apiKey = user.ApiKey });
    }
}
