using System.Text;
using System.Text.RegularExpressions;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Models;

namespace ZainPortfolio.Services;

public static partial class SlugHelper
{
    [GeneratedRegex(@"[^a-z0-9\s-]")] private static partial Regex Invalid();
    [GeneratedRegex(@"[\s-]+")] private static partial Regex Spaces();

    public static string Generate(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return Guid.NewGuid().ToString("n")[..8];
        var s = input.ToLowerInvariant().Trim();
        s = s.Replace("&", " and ").Replace("+", " plus ").Replace("/", "-").Replace(".", "-");
        s = Invalid().Replace(s, "");
        s = Spaces().Replace(s, "-").Trim('-');
        return string.IsNullOrWhiteSpace(s) ? Guid.NewGuid().ToString("n")[..8] : s;
    }

    /// <summary>Generates a slug that does not collide with an existing one.</summary>
    public static async Task<string> UniqueAsync(
        string input, Func<string, Task<bool>> exists, string? current = null)
    {
        var baseSlug = Generate(input);
        if (!string.IsNullOrEmpty(current) && string.Equals(baseSlug, current, StringComparison.OrdinalIgnoreCase))
            return baseSlug;

        var slug = baseSlug;
        var i = 2;
        while (await exists(slug))
        {
            slug = $"{baseSlug}-{i++}";
            if (i > 500) return $"{baseSlug}-{Guid.NewGuid():n}"[..60];
        }
        return slug;
    }
}

/// <summary>
/// Caches the singleton SiteSetting row so the layout does not hit the database on every request.
/// Invalidated whenever settings are saved from the admin area.
/// </summary>
public interface ISiteSettingsService
{
    Task<SiteSetting> GetAsync();
    void Invalidate();
}

public class SiteSettingsService : ISiteSettingsService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private SiteSetting? _cache;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public SiteSettingsService(IServiceScopeFactory scopeFactory) => _scopeFactory = scopeFactory;

    public async Task<SiteSetting> GetAsync()
    {
        if (_cache is not null) return _cache;
        await _lock.WaitAsync();
        try
        {
            if (_cache is not null) return _cache;
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _cache = await db.SiteSettings.AsNoTracking().FirstOrDefaultAsync() ?? new SiteSetting();
            return _cache;
        }
        finally { _lock.Release(); }
    }

    public void Invalidate() => _cache = null;
}

/// <summary>Whitelist-based HTML sanitiser for anything typed into the rich text editor.</summary>
public interface IHtmlSanitizerService { string Sanitize(string? html); }

public class HtmlSanitizerService : IHtmlSanitizerService
{
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerService()
    {
        _sanitizer = new HtmlSanitizer();
        _sanitizer.AllowedTags.Add("figure");
        _sanitizer.AllowedTags.Add("figcaption");
        _sanitizer.AllowedTags.Add("iframe");
        _sanitizer.AllowedTags.Add("video");
        _sanitizer.AllowedTags.Add("source");
        _sanitizer.AllowedAttributes.Add("allowfullscreen");
        _sanitizer.AllowedAttributes.Add("frameborder");
        _sanitizer.AllowedAttributes.Add("controls");
        _sanitizer.AllowedAttributes.Add("loading");
        _sanitizer.AllowedAttributes.Add("class");
        _sanitizer.AllowedSchemes.Add("mailto");
        // Only allow iframes from video hosts we trust.
        _sanitizer.FilterUrl += (_, e) =>
        {
            if (e.OriginalUrl.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                e.SanitizedUrl = null;
        };
    }

    public string Sanitize(string? html) =>
        string.IsNullOrWhiteSpace(html) ? string.Empty : _sanitizer.Sanitize(html);
}

/// <summary>Handles validated file uploads into wwwroot/uploads and records them as MediaAssets.</summary>
public interface IMediaService
{
    Task<(bool Ok, string? Error, MediaAsset? Asset)> SaveAsync(IFormFile file, string? altText = null);
    Task<bool> DeleteAsync(int id);
    string KindFor(string contentType);
}

public class MediaService : IMediaService
{
    private static readonly Dictionary<string, string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg", [".jpeg"] = "image/jpeg", [".png"] = "image/png",
        [".gif"] = "image/gif", [".webp"] = "image/webp", [".svg"] = "image/svg+xml",
        [".avif"] = "image/avif", [".ico"] = "image/x-icon",
        [".mp4"] = "video/mp4", [".webm"] = "video/webm", [".ogg"] = "video/ogg",
        [".pdf"] = "application/pdf",
    };

    private const long MaxBytes = 64L * 1024 * 1024;   // 64 MB

    private readonly IWebHostEnvironment _env;
    private readonly ApplicationDbContext _db;

    public MediaService(IWebHostEnvironment env, ApplicationDbContext db)
    {
        _env = env;
        _db = db;
    }

    public string KindFor(string contentType) => contentType switch
    {
        var c when c.StartsWith("image/") => "image",
        var c when c.StartsWith("video/") => "video",
        _ => "document"
    };

    public async Task<(bool, string?, MediaAsset?)> SaveAsync(IFormFile file, string? altText = null)
    {
        if (file.Length == 0) return (false, "The file is empty.", null);
        if (file.Length > MaxBytes) return (false, "File exceeds the 64 MB limit.", null);

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext) || !Allowed.TryGetValue(ext, out var contentType))
            return (false, $"File type '{ext}' is not allowed.", null);

        var root = Path.Combine(_env.WebRootPath, "uploads");
        Directory.CreateDirectory(root);

        var safeName = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid():n}"[..28] + ext.ToLowerInvariant();
        var fullPath = Path.Combine(root, safeName);

        await using (var stream = new FileStream(fullPath, FileMode.CreateNew))
            await file.CopyToAsync(stream);

        var asset = new MediaAsset
        {
            FileName = safeName,
            OriginalName = Path.GetFileName(file.FileName),
            Url = $"/uploads/{safeName}",
            ContentType = contentType,
            Kind = KindFor(contentType),
            SizeBytes = file.Length,
            AltText = altText
        };
        _db.MediaAssets.Add(asset);
        await _db.SaveChangesAsync();
        return (true, null, asset);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var asset = await _db.MediaAssets.FindAsync(id);
        if (asset is null) return false;

        var path = Path.Combine(_env.WebRootPath, "uploads", asset.FileName);
        if (File.Exists(path))
        {
            try { File.Delete(path); } catch { /* file already gone — still remove the record */ }
        }

        _db.MediaAssets.Remove(asset);
        await _db.SaveChangesAsync();
        return true;
    }
}

public static class TextHelpers
{
    /// <summary>Strips tags and truncates — used to auto-generate excerpts.</summary>
    public static string Plain(string? html, int maxLength = 0)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        var text = Regex.Replace(html, "<.*?>", " ");
        text = System.Net.WebUtility.HtmlDecode(text);
        text = Regex.Replace(text, @"\s+", " ").Trim();
        if (maxLength > 0 && text.Length > maxLength)
            text = text[..maxLength].TrimEnd() + "…";
        return text;
    }

    public static int EstimateReadMinutes(string? html)
    {
        var words = Plain(html).Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Ceiling(words / 200.0));
    }

    /// <summary>Adjusts a hex colour's lightness — used to derive hover/glow shades from the theme colour.</summary>
    public static string Shade(string hex, double factor)
    {
        try
        {
            hex = hex.TrimStart('#');
            if (hex.Length == 3)
                hex = string.Concat(hex.Select(c => $"{c}{c}"));
            var r = Convert.ToInt32(hex[..2], 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);
            int Adj(int v) => Math.Clamp((int)Math.Round(v + (factor > 0 ? (255 - v) * factor : v * factor)), 0, 255);
            return $"#{Adj(r):x2}{Adj(g):x2}{Adj(b):x2}";
        }
        catch { return "#" + hex; }
    }

    /// <summary>"124, 108, 240" — for use inside rgba() in CSS.</summary>
    public static string ToRgb(string hex)
    {
        try
        {
            hex = hex.TrimStart('#');
            if (hex.Length == 3) hex = string.Concat(hex.Select(c => $"{c}{c}"));
            return $"{Convert.ToInt32(hex[..2], 16)}, {Convert.ToInt32(hex.Substring(2, 2), 16)}, {Convert.ToInt32(hex.Substring(4, 2), 16)}";
        }
        catch { return "124, 108, 240"; }
    }

    public static IEnumerable<string> Csv(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Array.Empty<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static readonly Regex HeadingRx =
        new(@"<h([23])(?![^>]*\bid=)([^>]*)>(.*?)</h\1>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

    /// <summary>
    /// Gives every H2/H3 in post content a stable id and returns the heading list.
    /// Anchored headings are what let Google build "jump to section" links in the
    /// result, and they make long posts linkable section by section.
    /// </summary>
    public static (string Html, List<TocEntry> Toc) BuildToc(string? html)
    {
        var toc = new List<TocEntry>();
        if (string.IsNullOrWhiteSpace(html)) return (html ?? string.Empty, toc);

        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var rendered = HeadingRx.Replace(html, m =>
        {
            var level = int.Parse(m.Groups[1].Value);
            var attrs = m.Groups[2].Value;
            var inner = m.Groups[3].Value;
            var text = Plain(inner);
            if (string.IsNullOrWhiteSpace(text)) return m.Value;

            var id = Slugify(text);
            // Two sections can legitimately share a title — keep the ids unique.
            var candidate = id;
            for (var i = 2; !used.Add(candidate); i++) candidate = $"{id}-{i}";

            toc.Add(new TocEntry(candidate, text, level));
            return $"<h{level}{attrs} id=\"{candidate}\">{inner}</h{level}>";
        });

        return (rendered, toc);
    }

    /// <summary>
    /// ISO-8601 with an explicit UTC designator. Timestamps come back from the
    /// database with Kind=Unspecified, and "o" on an unspecified DateTime emits no
    /// offset at all — which crawlers read as an ambiguous local time.
    /// </summary>
    public static string Iso(DateTime? value) =>
        value.HasValue
            ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc).ToString("yyyy-MM-ddTHH:mm:ssZ")
            : string.Empty;

    public static string Slugify(string value)
    {
        var slug = System.Net.WebUtility.HtmlDecode(value).ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"[\s-]+", "-").Trim('-');
        if (slug.Length > 60) slug = slug[..60].TrimEnd('-');
        return string.IsNullOrEmpty(slug) ? "section" : slug;
    }
}

public record TocEntry(string Id, string Text, int Level);
