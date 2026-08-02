using System.Text.Json;
using ZainPortfolio.Models;
using ZainPortfolio.Models.ViewModels;

namespace ZainPortfolio.Services;

/// <summary>
/// Builds JSON-LD structured data. Rich results are the highest-leverage SEO win
/// for a personal brand site, and they are what earns the knowledge-panel style
/// presentation for "AI consultant Malaysia" style queries.
/// </summary>
public interface ISeoService
{
    string BaseUrl(HttpRequest request, SiteSetting settings);
    string PersonSchema(SiteSetting s, string baseUrl);
    string WebSiteSchema(SiteSetting s, string baseUrl);
    string ProfessionalServiceSchema(SiteSetting s, string baseUrl);
    string ArticleSchema(BlogPost post, SiteSetting s, string baseUrl);
    string CreativeWorkSchema(Project project, SiteSetting s, string baseUrl);
    string BreadcrumbSchema(IEnumerable<(string Name, string Url)> crumbs, string baseUrl);
    string FaqSchema(IEnumerable<(string Q, string A)> items);
}

public class SeoService : ISeoService
{
    private static readonly JsonSerializerOptions Json = new()
    {
        WriteIndented = false,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public string BaseUrl(HttpRequest request, SiteSetting settings)
    {
        if (!string.IsNullOrWhiteSpace(settings.CanonicalBaseUrl))
            return settings.CanonicalBaseUrl.TrimEnd('/');
        return $"{request.Scheme}://{request.Host}".TrimEnd('/');
    }

    private static string Ser(object o) => JsonSerializer.Serialize(o, Json);

    private static string? Abs(string? url, string baseUrl) =>
        string.IsNullOrWhiteSpace(url) ? null
        : url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url
        : baseUrl + "/" + url.TrimStart('/');

    public string PersonSchema(SiteSetting s, string baseUrl)
    {
        var sameAs = new[] { s.LinkedInUrl, s.GitHubUrl, s.YouTubeUrl, s.UpworkUrl, s.TwitterUrl }
            .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

        return Ser(new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "Person",
            ["@id"] = baseUrl + "/#person",
            ["name"] = s.OwnerName,
            ["url"] = baseUrl,
            ["image"] = Abs(s.ProfileImageUrl, baseUrl),
            ["jobTitle"] = s.Tagline,
            ["description"] = s.MetaDescription,
            ["email"] = "mailto:" + s.Email,
            ["telephone"] = s.Phone,
            ["address"] = new Dictionary<string, object?>
            {
                ["@type"] = "PostalAddress",
                ["addressLocality"] = "Kuala Lumpur",
                ["addressRegion"] = "Federal Territory of Kuala Lumpur",
                ["addressCountry"] = "MY"
            },
            ["knowsAbout"] = new[]
            {
                "Retrieval-Augmented Generation", "Large Language Models", "Agentic AI",
                "Microsoft Azure", "Cloud Architecture", ".NET Core", "Angular",
                "Microservices", "Engineering Leadership", "MLOps", "Azure OpenAI"
            },
            ["knowsLanguage"] = new[] { "en", "ur", "ms" },
            ["sameAs"] = sameAs
        });
    }

    public string WebSiteSchema(SiteSetting s, string baseUrl) => Ser(new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "WebSite",
        ["@id"] = baseUrl + "/#website",
        ["url"] = baseUrl,
        ["name"] = s.SiteName,
        ["description"] = s.MetaDescription,
        ["inLanguage"] = s.ContentLanguage,
        ["publisher"] = new Dictionary<string, object?> { ["@id"] = baseUrl + "/#person" },
        ["potentialAction"] = new Dictionary<string, object?>
        {
            ["@type"] = "SearchAction",
            ["target"] = new Dictionary<string, object?>
            {
                ["@type"] = "EntryPoint",
                ["urlTemplate"] = baseUrl + "/blog?q={search_term_string}"
            },
            ["query-input"] = "required name=search_term_string"
        }
    });

    /// <summary>Local/regional signal — this is what targets Malaysia specifically.</summary>
    public string ProfessionalServiceSchema(SiteSetting s, string baseUrl) => Ser(new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "ProfessionalService",
        ["@id"] = baseUrl + "/#service",
        ["name"] = $"{s.OwnerName} — AI & Cloud Architecture Consulting",
        ["description"] = "AI, RAG/LLM platform and Azure cloud architecture consulting for enterprises in Malaysia and Southeast Asia.",
        ["url"] = baseUrl,
        ["image"] = Abs(s.ProfileImageUrl ?? s.OgImageUrl, baseUrl),
        ["telephone"] = s.Phone,
        ["email"] = s.Email,
        ["priceRange"] = "$$$",
        ["address"] = new Dictionary<string, object?>
        {
            ["@type"] = "PostalAddress",
            ["addressLocality"] = "Kuala Lumpur",
            ["addressCountry"] = "MY"
        },
        ["geo"] = new Dictionary<string, object?>
        {
            ["@type"] = "GeoCoordinates",
            ["latitude"] = s.GeoPosition.Split(';').FirstOrDefault(),
            ["longitude"] = s.GeoPosition.Split(';').Skip(1).FirstOrDefault()
        },
        ["areaServed"] = s.ServiceArea.Split(',', StringSplitOptions.TrimEntries)
            .Select(a => new Dictionary<string, object?> { ["@type"] = "Place", ["name"] = a }).ToArray(),
        ["provider"] = new Dictionary<string, object?> { ["@id"] = baseUrl + "/#person" },
        ["hasOfferCatalog"] = new Dictionary<string, object?>
        {
            ["@type"] = "OfferCatalog",
            ["name"] = "Consulting Services",
            ["itemListElement"] = new[]
            {
                "AI & RAG Platform Architecture", "Azure Cloud Architecture & Migration",
                "LLM Application Development", "Engineering Team Leadership",
                ".NET & Angular Enterprise Development", "DevOps & CI/CD Implementation"
            }.Select(n => new Dictionary<string, object?>
            {
                ["@type"] = "Offer",
                ["itemOffered"] = new Dictionary<string, object?> { ["@type"] = "Service", ["name"] = n }
            }).ToArray()
        }
    });

    public string ArticleSchema(BlogPost p, SiteSetting s, string baseUrl) => Ser(new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "BlogPosting",
        ["@id"] = $"{baseUrl}/blog/{p.Slug}#article",
        ["headline"] = p.Title.Length > 110 ? p.Title[..110] : p.Title,
        ["description"] = p.MetaDescription ?? p.Excerpt,
        ["image"] = Abs(p.CoverImageUrl ?? s.OgImageUrl, baseUrl),
        ["datePublished"] = p.PublishedAt.ToString("o"),
        ["dateModified"] = p.UpdatedAt.ToString("o"),
        ["wordCount"] = TextHelpers.Plain(p.Content).Split(' ', StringSplitOptions.RemoveEmptyEntries).Length,
        ["timeRequired"] = $"PT{p.ReadMinutes}M",
        ["inLanguage"] = s.ContentLanguage,
        ["keywords"] = p.Tags,
        ["articleSection"] = p.BlogCategory?.Name,
        ["author"] = new Dictionary<string, object?> { ["@id"] = baseUrl + "/#person" },
        ["publisher"] = new Dictionary<string, object?> { ["@id"] = baseUrl + "/#person" },
        ["mainEntityOfPage"] = new Dictionary<string, object?>
        {
            ["@type"] = "WebPage",
            ["@id"] = $"{baseUrl}/blog/{p.Slug}"
        }
    });

    public string CreativeWorkSchema(Project p, SiteSetting s, string baseUrl) => Ser(new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "CreativeWork",
        ["@id"] = $"{baseUrl}/projects/{p.Slug}#project",
        ["name"] = p.Title,
        ["description"] = p.Summary,
        ["image"] = Abs(p.CoverImageUrl ?? s.OgImageUrl, baseUrl),
        ["url"] = $"{baseUrl}/projects/{p.Slug}",
        ["dateCreated"] = p.CreatedAt.ToString("o"),
        ["keywords"] = p.Tags,
        ["inLanguage"] = s.ContentLanguage,
        ["creator"] = new Dictionary<string, object?> { ["@id"] = baseUrl + "/#person" },
        ["about"] = p.TagList.Select(t => new Dictionary<string, object?>
        {
            ["@type"] = "Thing", ["name"] = t
        }).ToArray()
    });

    public string BreadcrumbSchema(IEnumerable<(string Name, string Url)> crumbs, string baseUrl)
    {
        var items = crumbs.Select((c, i) => new Dictionary<string, object?>
        {
            ["@type"] = "ListItem",
            ["position"] = i + 1,
            ["name"] = c.Name,
            ["item"] = c.Url.StartsWith("http") ? c.Url : baseUrl + "/" + c.Url.TrimStart('/')
        }).ToArray();

        return Ser(new Dictionary<string, object?>
        {
            ["@context"] = "https://schema.org",
            ["@type"] = "BreadcrumbList",
            ["itemListElement"] = items
        });
    }

    public string FaqSchema(IEnumerable<(string Q, string A)> items) => Ser(new Dictionary<string, object?>
    {
        ["@context"] = "https://schema.org",
        ["@type"] = "FAQPage",
        ["mainEntity"] = items.Select(i => new Dictionary<string, object?>
        {
            ["@type"] = "Question",
            ["name"] = i.Q,
            ["acceptedAnswer"] = new Dictionary<string, object?> { ["@type"] = "Answer", ["text"] = i.A }
        }).ToArray()
    });
}
