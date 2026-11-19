using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;

namespace ZainPortfolio.Services;

public record ChatLink(string Text, string Url);

public record ChatAnswer
{
    public string Message { get; init; } = "";
    public List<ChatLink> Links { get; init; } = new();
    public List<string> Suggestions { get; init; } = new();
}

public interface IChatbotService
{
    Task<ChatAnswer> AskAsync(string question);
    Task<List<string>> StarterQuestionsAsync();
}

/// <summary>
/// Answers visitor questions from live CMS content — projects, posts, skills,
/// experience and settings. Intent matching first, then a content search fallback,
/// so answers stay correct automatically as the site's content changes.
/// </summary>
public class ChatbotService : IChatbotService
{
    private readonly ApplicationDbContext _db;
    private readonly ISiteSettingsService _settings;

    public ChatbotService(ApplicationDbContext db, ISiteSettingsService settings)
    {
        _db = db;
        _settings = settings;
    }

    public Task<List<string>> StarterQuestionsAsync() => Task.FromResult(new List<string>
    {
        "What do you do?",
        "Show me your AI projects",
        "What's your experience?",
        "Which technologies do you use?",
        "Are you available for work?",
        "How can I contact you?"
    });

    private static readonly string[] Greetings = { "hi", "hello", "hey", "salam", "assalam", "good morning", "good afternoon", "good evening", "yo" };
    private static readonly string[] Thanks = { "thank", "thanks", "appreciate", "cheers" };
    private static readonly string[] Byes = { "bye", "goodbye", "see you", "later" };

    public async Task<ChatAnswer> AskAsync(string question)
    {
        var s = await _settings.GetAsync();
        var q = (question ?? "").Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(q))
            return new ChatAnswer { Message = "Ask me anything about Zain's work, experience or availability.", Suggestions = await StarterQuestionsAsync() };

        bool Has(params string[] terms) => terms.Any(t => q.Contains(t));

        // ---------------------------------------------------------- social
        if (Greetings.Any(g => q == g || q.StartsWith(g + " ") || q.StartsWith(g + ",")))
            return new ChatAnswer
            {
                Message = $"Hello. I'm {s.OwnerName}'s assistant — I can answer questions about his projects, experience, skills and availability using what's on this site.",
                Suggestions = await StarterQuestionsAsync()
            };

        if (Has(Thanks))
            return new ChatAnswer
            {
                Message = "You're welcome. If you'd like to discuss a project, the contact form below is the fastest route.",
                Links = { new ChatLink("Get in touch", "/#contact") }
            };

        if (Has(Byes))
            return new ChatAnswer { Message = "Thanks for visiting — feel free to reach out any time." };

        // ---------------------------------------------------------- contact
        if (Has("contact", "email", "reach", "get in touch", "whatsapp", "phone", "call", "hire", "book", "meeting", "schedule"))
        {
            var links = new List<ChatLink> { new("Email " + s.Email, "mailto:" + s.Email) };
            if (!string.IsNullOrWhiteSpace(s.WhatsAppNumber)) links.Add(new("WhatsApp", $"https://wa.me/{s.WhatsAppNumber}"));
            if (!string.IsNullOrWhiteSpace(s.CalendlyUrl)) links.Add(new("Book a call", s.CalendlyUrl));
            if (!string.IsNullOrWhiteSpace(s.LinkedInUrl)) links.Add(new("LinkedIn", s.LinkedInUrl));
            return new ChatAnswer
            {
                Message = $"The quickest way to reach {s.OwnerName} is email at {s.Email}. He's based in {s.Location}.",
                Links = links,
                Suggestions = { "Are you available for work?", "What do you charge?" }
            };
        }

        // ---------------------------------------------------------- availability
        if (Has("available", "availability", "freelance", "open to", "looking for", "job", "opportunit", "remote", "relocat"))
            return new ChatAnswer
            {
                Message = $"{s.Availability}. Based in {s.Location}, working with clients across {s.ServiceArea}.",
                Links = { new ChatLink("Start a conversation", "/#contact") },
                Suggestions = { "How can I contact you?", "What's your experience?" }
            };

        // ---------------------------------------------------------- rates
        if (Has("rate", "cost", "price", "pricing", "charge", "salary", "budget", "how much"))
            return new ChatAnswer
            {
                Message = "Rates depend on scope, duration and engagement model, so it's best discussed directly. Send a short note about what you're building and you'll get a considered answer rather than a generic number.",
                Links = { new ChatLink("Discuss your project", "/#contact") }
            };

        // ---------------------------------------------------------- location
        if (Has("where", "location", "based", "country", "city", "timezone", "time zone"))
            return new ChatAnswer
            {
                Message = $"{s.OwnerName} is based in {s.Location}, and works with clients across {s.ServiceArea}.",
                Suggestions = { "Are you available for work?" }
            };

        // ---------------------------------------------------------- experience
        if (Has("experience", "background", "career", "worked", "history", "cv", "resume", "years", "companies", "employer"))
        {
            var roles = await _db.Experiences.AsNoTracking().OrderBy(e => e.SortOrder).Take(4).ToListAsync();
            var lines = roles.Select(r => $"• {r.Role} at {r.Company} ({r.DateRange})");
            return new ChatAnswer
            {
                Message = $"{s.StatYears}+ years in enterprise software, most recently:\n\n{string.Join("\n", lines)}",
                Links = { new ChatLink("See the full timeline", "/#experience") },
                Suggestions = { "Show me your AI projects", "Which technologies do you use?" }
            };
        }

        // ---------------------------------------------------------- skills
        if (Has("skill", "technolog", "tech stack", "stack", "tools", "language", "framework", "expertise", "know", "proficien"))
        {
            var cats = await _db.SkillCategories.AsNoTracking()
                .Include(c => c.Skills.OrderByDescending(x => x.Percentage))
                .OrderBy(c => c.SortOrder).ToListAsync();
            var lines = cats.Select(c => $"• {c.Name}: {string.Join(", ", c.Skills.Take(4).Select(x => x.Name))}");
            return new ChatAnswer
            {
                Message = "Core areas:\n\n" + string.Join("\n", lines),
                Links = { new ChatLink("See proficiency levels", "/#skills") },
                Suggestions = { "Show me your AI projects", "What's your experience?" }
            };
        }

        // ---------------------------------------------------------- about
        // Deliberately narrow: "tell me about azure" is a topic search, not a bio request,
        // so only phrases pointing at the person himself match here.
        if (Has("who are you", "about you", "about yourself", "about zain", "introduce",
                "what do you do", "yourself", "your bio", "who is zain"))
            return new ChatAnswer
            {
                Message = $"{s.OwnerName} — {s.Tagline}. {s.AboutLead}",
                Links = { new ChatLink("Read more", "/#about"), new ChatLink("Watch the intro", "/#home") },
                Suggestions = { "Show me your AI projects", "What's your experience?" }
            };

        // ---------------------------------------------------------- blog
        if (Has("blog", "article", "post", "writing", "read", "insight"))
        {
            var posts = await _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished).OrderByDescending(p => p.PublishedAt).Take(3).ToListAsync();
            var answer = new ChatAnswer
            {
                Message = posts.Any()
                    ? "Recent writing on AI architecture, Azure and engineering leadership:"
                    : "There are no published articles yet — check back shortly.",
                Suggestions = { "Show me your AI projects" }
            };
            foreach (var p in posts) answer.Links.Add(new ChatLink(p.Title, "/blog/" + p.Slug));
            if (posts.Any()) answer.Links.Add(new ChatLink("All articles", "/blog"));
            return answer;
        }

        // ---------------------------------------------------------- projects (with content search)
        var projectIntent = Has("project", "work", "portfolio", "built", "build", "case study", "example", "show me", "done");
        var searchable = q.Length > 2;

        if (projectIntent || searchable)
        {
            var terms = q.Split(new[] { ' ', ',', '?', '.', '!' }, StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 3 && !Stop.Contains(w))
                .Distinct().Take(6).ToList();

            if (terms.Any())
            {
                var candidates = await _db.Projects.AsNoTracking()
                    .Where(p => p.IsPublished)
                    .Select(p => new { p.Title, p.Slug, p.Summary, p.Tags, p.Problem, p.Solution })
                    .ToListAsync();

                var ranked = candidates
                    .Select(p => new
                    {
                        p.Title,
                        p.Slug,
                        p.Summary,
                        Score = terms.Sum(t =>
                            (p.Title.Contains(t, StringComparison.OrdinalIgnoreCase) ? 4 : 0) +
                            ((p.Tags ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ? 3 : 0) +
                            ((p.Summary ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ? 2 : 0) +
                            ((p.Problem ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ? 1 : 0) +
                            ((p.Solution ?? "").Contains(t, StringComparison.OrdinalIgnoreCase) ? 1 : 0))
                    })
                    .Where(x => x.Score > 0)
                    .OrderByDescending(x => x.Score)
                    .Take(3).ToList();

                if (ranked.Any())
                {
                    var top = ranked.First();
                    var answer = new ChatAnswer
                    {
                        Message = ranked.Count == 1
                            ? $"**{top.Title}** — {top.Summary}"
                            : $"Found {ranked.Count} relevant projects. The closest match is **{top.Title}** — {top.Summary}",
                        Suggestions = { "Show me your AI projects", "How can I contact you?" }
                    };
                    foreach (var r in ranked) answer.Links.Add(new ChatLink(r.Title, "/projects/" + r.Slug));
                    return answer;
                }
            }
        }

        if (projectIntent)
        {
            var featured = await _db.Projects.AsNoTracking()
                .Where(p => p.IsPublished && p.IsFeatured)
                .OrderBy(p => p.SortOrder).Take(4).ToListAsync();
            var total = await _db.Projects.CountAsync(p => p.IsPublished);

            var answer = new ChatAnswer
            {
                Message = $"There are {total} projects on the site, each written up as a problem, a solution and a measured outcome. A few highlights:",
                Suggestions = { "Which technologies do you use?", "How can I contact you?" }
            };
            foreach (var p in featured) answer.Links.Add(new ChatLink(p.Title, "/projects/" + p.Slug));
            answer.Links.Add(new ChatLink("Browse all projects", "/projects"));
            return answer;
        }

        // ---------------------------------------------------------- fallback
        return new ChatAnswer
        {
            Message = "I'm not sure about that one. I can help with skills, experience, projects, articles, availability or how to get in touch — or send a message directly and Zain will reply personally.",
            Links = { new ChatLink("Send a message", "/#contact") },
            Suggestions = await StarterQuestionsAsync()
        };
    }

    private static readonly HashSet<string> Stop = new(StringComparer.OrdinalIgnoreCase)
    {
        "what", "which", "where", "when", "have", "your", "you", "the", "and", "for", "with",
        "does", "did", "can", "could", "would", "about", "some", "any", "show", "tell", "give",
        "there", "that", "this", "from", "into", "been", "were", "they", "them", "their", "please"
    };
}
