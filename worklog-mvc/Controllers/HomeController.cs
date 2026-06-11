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
}
