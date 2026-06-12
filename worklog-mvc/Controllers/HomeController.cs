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
        ViewData["Description"] = "WorkLog is a simple, AI-powered work logging tool for individuals and small teams. Log what you worked on, track hours, and get daily summaries.";
        return View();
    }

    public IActionResult Privacy()
    {
        ViewData["Title"] = "Privacy Policy — WorkLog";
        ViewData["Description"] = "WorkLog Privacy Policy — learn how we collect, use, and protect your personal information.";
        return View();
    }

    public IActionResult Terms()
    {
        ViewData["Title"] = "Terms of Service — WorkLog";
        ViewData["Description"] = "WorkLog Terms of Service — the rules and conditions for using the WorkLog application.";
        return View();
    }

    public IActionResult Contact()
    {
        ViewData["Title"] = "Contact — WorkLog";
        ViewData["Description"] = "Get in touch with the WorkLog team. We'd love to hear your feedback, feature requests, or questions.";
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult ContactSubmit(string name, string email, string subject, string message)
    {
        // In production: send email or store in DB. For now, show success toast.
        TempData["ContactSent"] = "sent";
        return RedirectToAction(nameof(Contact));
    }
}
