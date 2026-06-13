using Microsoft.AspNetCore.Mvc;

namespace WorkLogApp.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Logs");
        return View();
    }

    public IActionResult About()
    {
        ViewData["Title"] = "About — WorkLog";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy — WorkLog";
        return View();
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms of Service — WorkLog";
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Title"] = "Contact — WorkLog";
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult ContactSubmit(string name, string email, string subject, string message)
    {
        TempData["ContactSent"] = "sent";
        return RedirectToAction(nameof(Contact));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        Response.StatusCode = 500;
        return View();
    }

    public new IActionResult NotFound()
    {
        Response.StatusCode = 404;
        return View();
    }
}
