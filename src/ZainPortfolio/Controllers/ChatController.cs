using Microsoft.AspNetCore.Mvc;
using ZainPortfolio.Services;

namespace ZainPortfolio.Controllers;

[Route("chat")]
public class ChatController : Controller
{
    private readonly IChatbotService _bot;
    private readonly ISiteSettingsService _settings;

    public ChatController(IChatbotService bot, ISiteSettingsService settings)
    {
        _bot = bot;
        _settings = settings;
    }

    [HttpPost("ask")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ask(string question)
    {
        var s = await _settings.GetAsync();
        if (!s.EnableChatbot) return NotFound();

        if (question is { Length: > 500 })
            question = question[..500];

        var answer = await _bot.AskAsync(question ?? "");
        return Json(new
        {
            message = answer.Message,
            links = answer.Links.Select(l => new { text = l.Text, url = l.Url }),
            suggestions = answer.Suggestions
        });
    }
}
