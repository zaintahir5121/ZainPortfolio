using System.Text;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Services;

namespace ZainPortfolio.Controllers;

/// <summary>Serves sitemap.xml, robots.txt and the RSS feed straight from the database.</summary>
public class SeoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly ISiteSettingsService _settings;
    private readonly ISeoService _seo;

    public SeoController(ApplicationDbContext db, ISiteSettingsService settings, ISeoService seo)
    {
        _db = db;
        _settings = settings;
        _seo = seo;
    }

    [ResponseCache(Duration = 3600)]
    [Route("/sitemap.xml")]
    public async Task<IActionResult> Sitemap()
    {
        var s = await _settings.GetAsync();
        var baseUrl = _seo.BaseUrl(Request, s);
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var doc = new XDocument(new XElement(ns + "urlset"));
        var root = doc.Root!;

        void Add(string path, DateTime? lastMod, string freq, string priority)
        {
            var el = new XElement(ns + "url",
                new XElement(ns + "loc", $"{baseUrl}/{path.TrimStart('/')}".TrimEnd('/')),
                new XElement(ns + "changefreq", freq),
                new XElement(ns + "priority", priority));
            if (lastMod.HasValue)
                el.Add(new XElement(ns + "lastmod", lastMod.Value.ToString("yyyy-MM-dd")));
            root.Add(el);
        }

        Add("", DateTime.UtcNow, "weekly", "1.0");

        if (s.EnableProjects)
        {
            Add("projects", DateTime.UtcNow, "weekly", "0.9");
            var projects = await _db.Projects.AsNoTracking()
                .Where(p => p.IsPublished)
                .Select(p => new { p.Slug, p.UpdatedAt }).ToListAsync();
            foreach (var p in projects)
                Add($"projects/{p.Slug}", p.UpdatedAt, "monthly", "0.8");
        }

        if (s.EnableBlog)
        {
            Add("blog", DateTime.UtcNow, "daily", "0.9");
            var posts = await _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished)
                .Select(p => new { p.Slug, p.UpdatedAt }).ToListAsync();
            foreach (var p in posts)
                Add($"blog/{p.Slug}", p.UpdatedAt, "monthly", "0.8");

            var cats = await _db.BlogCategories.AsNoTracking().Select(c => c.Slug).ToListAsync();
            foreach (var c in cats)
                Add($"blog?category={c}", null, "weekly", "0.5");

            // Tag pages are real long-tail landing pages, but a tag with a single
            // post is a thin duplicate of that post — only submit tags that
            // actually aggregate something.
            var tagLists = await _db.BlogPosts.AsNoTracking()
                .Where(p => p.IsPublished && p.Tags != null)
                .Select(p => p.Tags!).ToListAsync();

            var tags = tagLists
                .SelectMany(TextHelpers.Csv)
                .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
                .Where(g => g.Count() >= 2)
                .Select(g => g.Key)
                .OrderBy(t => t);

            foreach (var t in tags)
                Add($"blog?tag={Uri.EscapeDataString(t)}", null, "weekly", "0.4");
        }

        var xml = new StringBuilder();
        xml.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
        xml.Append(doc.ToString(SaveOptions.DisableFormatting));
        return Content(xml.ToString(), "application/xml", Encoding.UTF8);
    }

    [ResponseCache(Duration = 86400)]
    [Route("/robots.txt")]
    public async Task<IActionResult> Robots()
    {
        var s = await _settings.GetAsync();
        var baseUrl = _seo.BaseUrl(Request, s);

        var sb = new StringBuilder();
        sb.AppendLine("User-agent: *");
        sb.AppendLine("Allow: /");
        sb.AppendLine("Disallow: /admin");
        sb.AppendLine("Disallow: /admin/");
        sb.AppendLine("Disallow: /error");
        sb.AppendLine();
        sb.AppendLine("# AI crawlers — allowed, this site benefits from being cited");
        sb.AppendLine("User-agent: GPTBot");
        sb.AppendLine("Allow: /");
        sb.AppendLine();
        sb.AppendLine($"Sitemap: {baseUrl}/sitemap.xml");
        return Content(sb.ToString(), "text/plain", Encoding.UTF8);
    }

    [ResponseCache(Duration = 1800)]
    [Route("/rss.xml")]
    [Route("/feed")]
    public async Task<IActionResult> Rss()
    {
        var s = await _settings.GetAsync();
        var baseUrl = _seo.BaseUrl(Request, s);

        var posts = await _db.BlogPosts.AsNoTracking()
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.PublishedAt)
            .Take(30).ToListAsync();

        var items = posts.Select(p => new XElement("item",
            new XElement("title", p.Title),
            new XElement("link", $"{baseUrl}/blog/{p.Slug}"),
            new XElement("guid", new XAttribute("isPermaLink", "true"), $"{baseUrl}/blog/{p.Slug}"),
            new XElement("description", p.Excerpt ?? TextHelpers.Plain(p.Content, 300)),
            new XElement("pubDate", p.PublishedAt.ToString("r"))));

        var doc = new XDocument(
            new XElement("rss", new XAttribute("version", "2.0"),
                new XElement("channel",
                    new XElement("title", s.SiteName + " — Blog"),
                    new XElement("link", baseUrl + "/blog"),
                    new XElement("description", s.MetaDescription ?? s.Tagline),
                    new XElement("language", s.ContentLanguage.ToLowerInvariant()),
                    new XElement("lastBuildDate", DateTime.UtcNow.ToString("r")),
                    items)));

        return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + doc.ToString(SaveOptions.DisableFormatting),
            "application/rss+xml", Encoding.UTF8);
    }
}
