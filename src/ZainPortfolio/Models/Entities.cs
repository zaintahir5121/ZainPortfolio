using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZainPortfolio.Models;

/// <summary>
/// Singleton row (Id = 1) holding every branding, theme, contact and SEO value
/// that the public site renders. Editable end-to-end from Admin → Settings.
/// </summary>
public class SiteSetting
{
    public int Id { get; set; }

    // ---- Identity / branding ----
    [Required, MaxLength(120)] public string SiteName { get; set; } = "Zain Abbas Tahir";
    [MaxLength(200)] public string Tagline { get; set; } = "AI Technical Lead & Cloud Architect";
    [MaxLength(120)] public string OwnerName { get; set; } = "Zain Abbas Tahir";
    [MaxLength(30)] public string LogoText { get; set; } = "ZAT";
    [MaxLength(400)] public string? LogoImageUrl { get; set; }
    [MaxLength(400)] public string? FaviconUrl { get; set; }

    // ---- Theme ----
    [MaxLength(20)] public string PrimaryColor { get; set; } = "#7c6cf0";
    [MaxLength(20)] public string SecondaryColor { get; set; } = "#a78bfa";
    [MaxLength(20)] public string AccentColor { get; set; } = "#22d3ee";
    [MaxLength(20)] public string DarkBgColor { get; set; } = "#0b0d16";
    [MaxLength(20)] public string LightBgColor { get; set; } = "#f7f8fc";
    [MaxLength(10)] public string DefaultTheme { get; set; } = "dark";   // dark | light
    [MaxLength(80)] public string HeadingFont { get; set; } = "Inter";
    [MaxLength(80)] public string BodyFont { get; set; } = "Inter";
    [MaxLength(20)] public string CornerRadius { get; set; } = "16px";

    // ---- Hero ----
    [MaxLength(160)] public string HeroBadge { get; set; } = "Available for new opportunities";
    [MaxLength(200)] public string HeroHeadingLine1 { get; set; } = "Building AI-Powered";
    [MaxLength(200)] public string HeroHeadingHighlight { get; set; } = "AI-Powered";
    [MaxLength(200)] public string HeroHeadingLine2 { get; set; } = "Systems That Scale";
    [MaxLength(1000)] public string HeroSubheading { get; set; } =
        "14+ years designing enterprise-grade solutions with RAG, LLMs, Azure Cloud, .NET Core & Angular.";
    [MaxLength(400)] public string TypewriterRoles { get; set; } =
        "AI Technical Lead,Cloud Architect,Engineering Manager,Solution Architect";
    [MaxLength(120)] public string? IntroVideoId { get; set; } = "DNv6xTIlyIg";
    public bool IntroVideoAutoplay { get; set; } = true;

    // ---- Stats ----
    public int StatYears { get; set; } = 14;
    public int StatProjects { get; set; } = 60;
    public int StatEnterprise { get; set; } = 20;
    [MaxLength(60)] public string StatYearsLabel { get; set; } = "Years Exp.";
    [MaxLength(60)] public string StatProjectsLabel { get; set; } = "Projects";
    [MaxLength(60)] public string StatEnterpriseLabel { get; set; } = "Enterprise";

    // ---- About ----
    [MaxLength(200)] public string AboutTitle { get; set; } = "Turning Data Into Intelligence";
    [MaxLength(120)] public string AboutTitleHighlight { get; set; } = "Intelligence";
    public string AboutLead { get; set; } =
        "I build intelligent, scalable, and predictive systems — designing advanced AI architectures using RAG, LLMs, Predictive AI and AI Foundry platforms.";
    public string AboutBody { get; set; } =
        "My approach combines microservices, event-driven design, and AI-powered automation to deliver enterprise-scale, cloud-native solutions.";
    [MaxLength(400)] public string? ProfileImageUrl { get; set; }
    [MaxLength(1200)] public string AboutTags { get; set; } =
        "Custom Software Development,API Development & Integration,Cloud Solutions & Azure,Microservices Architecture,Performance Optimization,DevOps & CI/CD Setup";

    // ---- Contact ----
    [MaxLength(200)] public string Email { get; set; } = "zain.tahir512@gmail.com";
    [MaxLength(60)] public string? Phone { get; set; } = "+60 10-363-5921";
    [MaxLength(60)] public string? WhatsAppNumber { get; set; } = "60103635921";
    [MaxLength(160)] public string Location { get; set; } = "Kuala Lumpur, Malaysia";
    [MaxLength(200)] public string Availability { get; set; } = "Open to remote & freelance work";

    // ---- Social ----
    [MaxLength(400)] public string? LinkedInUrl { get; set; } = "https://www.linkedin.com/in/zainabbastahir/";
    [MaxLength(400)] public string? GitHubUrl { get; set; } = "https://github.com/zainabbastahir";
    [MaxLength(400)] public string? YouTubeUrl { get; set; } = "https://www.youtube.com/@zainabbastahir";
    [MaxLength(400)] public string? UpworkUrl { get; set; }
    [MaxLength(400)] public string? CalendlyUrl { get; set; }
    [MaxLength(400)] public string? TwitterUrl { get; set; }

    // ---- Feature switches ----
    public bool EnableBlog { get; set; } = true;
    public bool EnableProjects { get; set; } = true;
    public bool EnableChatbot { get; set; } = true;
    public bool EnableWhatsAppFloat { get; set; } = true;
    public bool EnableContactForm { get; set; } = true;
    public bool EnableParticles { get; set; } = true;

    public bool EnableNewsletter { get; set; } = true;
    public bool EnableAnalytics { get; set; } = true;

    // ---- SEO ----
    [MaxLength(70)] public string? MetaTitle { get; set; } =
        "Zain Abbas Tahir | AI Technical Lead & Cloud Architect in Kuala Lumpur, Malaysia";
    [MaxLength(300)] public string? MetaDescription { get; set; } =
        "AI Technical Lead & Cloud Architect in Kuala Lumpur, Malaysia. 14+ years building RAG, LLM, Azure cloud and .NET enterprise solutions for teams across Malaysia and Asia.";
    // Long-tail, low-competition terms beat generic heads like "AI consultant".
    [MaxLength(500)] public string? MetaKeywords { get; set; } =
        "RAG implementation consultant Malaysia, Azure OpenAI consultant Kuala Lumpur, LLM integration services Malaysia, agentic AI developer Malaysia, enterprise RAG chatbot Kuala Lumpur, Copilot Studio consultant Malaysia, .NET Core Azure architect Malaysia, AI compliance automation Malaysia, hire AI technical lead Kuala Lumpur";
    [MaxLength(60)] public string? GoogleAnalyticsId { get; set; }
    [MaxLength(120)] public string? GoogleSiteVerification { get; set; }
    [MaxLength(120)] public string? BingSiteVerification { get; set; }
    [MaxLength(400)] public string? OgImageUrl { get; set; }
    [MaxLength(60)] public string? TwitterHandle { get; set; }

    /// <summary>Canonical site origin, e.g. https://zainabbastahir.com — required for correct sitemap and og:url values.</summary>
    [MaxLength(200)] public string? CanonicalBaseUrl { get; set; } = "https://zainabbastahir.com";

    // Regional targeting (Malaysia)
    [MaxLength(10)] public string GeoRegion { get; set; } = "MY-14";              // Kuala Lumpur
    [MaxLength(120)] public string GeoPlacename { get; set; } = "Kuala Lumpur, Malaysia";
    [MaxLength(60)] public string GeoPosition { get; set; } = "3.139003;101.686855";
    [MaxLength(10)] public string ContentLanguage { get; set; } = "en-MY";
    [MaxLength(120)] public string ServiceArea { get; set; } = "Malaysia, Singapore, Southeast Asia";

    [MaxLength(300)] public string FooterText { get; set; } = "All rights reserved.";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class ProjectCategory
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    [Required, MaxLength(80)] public string Slug { get; set; } = "";
    public int SortOrder { get; set; }
    public ICollection<Project> Projects { get; set; } = new List<Project>();
}

public class Project
{
    public int Id { get; set; }
    [Required, MaxLength(200)] public string Title { get; set; } = "";
    [Required, MaxLength(220)] public string Slug { get; set; } = "";
    [MaxLength(500)] public string? Summary { get; set; }

    [MaxLength(1500)] public string? Problem { get; set; }
    [MaxLength(1500)] public string? Solution { get; set; }
    /// <summary>Full case-study body (sanitised HTML from the rich text editor).</summary>
    public string? Content { get; set; }

    // Visuals — either a cover image, or an icon + gradient tile
    [MaxLength(400)] public string? CoverImageUrl { get; set; }
    [MaxLength(80)] public string IconClass { get; set; } = "fas fa-cube";
    [MaxLength(20)] public string GradientFrom { get; set; } = "#0891b2";
    [MaxLength(20)] public string GradientTo { get; set; } = "#6366f1";
    [MaxLength(120)] public string? VideoId { get; set; }

    [MaxLength(400)] public string? LiveUrl { get; set; }
    [MaxLength(400)] public string? RepoUrl { get; set; }

    [MaxLength(500)] public string? Tags { get; set; }          // comma separated
    [MaxLength(120)] public string? Stat1Icon { get; set; }
    [MaxLength(80)] public string? Stat1Text { get; set; }
    [MaxLength(120)] public string? Stat2Icon { get; set; }
    [MaxLength(80)] public string? Stat2Text { get; set; }

    public int? ProjectCategoryId { get; set; }
    public ProjectCategory? ProjectCategory { get; set; }
    /// <summary>Extra filter keys, e.g. "ai cloud" — drives the public filter buttons.</summary>
    [MaxLength(160)] public string? FilterKeys { get; set; }

    public bool IsFeatured { get; set; }
    public bool IsPublished { get; set; } = true;
    public int SortOrder { get; set; }
    public int ViewCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProjectImage> Images { get; set; } = new List<ProjectImage>();

    [NotMapped] public IEnumerable<string> TagList =>
        string.IsNullOrWhiteSpace(Tags) ? Array.Empty<string>()
        : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

public class ProjectImage
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project? Project { get; set; }
    [Required, MaxLength(400)] public string Url { get; set; } = "";
    [MaxLength(200)] public string? Caption { get; set; }
    public int SortOrder { get; set; }
}

public class BlogCategory
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Name { get; set; } = "";
    [Required, MaxLength(80)] public string Slug { get; set; } = "";
    [MaxLength(200)] public string? Description { get; set; }
    public int SortOrder { get; set; }
    public ICollection<BlogPost> Posts { get; set; } = new List<BlogPost>();
}

public class BlogPost
{
    public int Id { get; set; }
    [Required, MaxLength(250)] public string Title { get; set; } = "";
    [Required, MaxLength(270)] public string Slug { get; set; } = "";
    [MaxLength(600)] public string? Excerpt { get; set; }
    /// <summary>Article body — sanitised HTML from the rich text editor.</summary>
    public string? Content { get; set; }

    [MaxLength(400)] public string? CoverImageUrl { get; set; }
    [MaxLength(120)] public string? VideoId { get; set; }

    [MaxLength(120)] public string AuthorName { get; set; } = "Zain Abbas Tahir";
    [MaxLength(400)] public string? AuthorImageUrl { get; set; }
    public int ReadMinutes { get; set; } = 5;
    [MaxLength(400)] public string? Tags { get; set; }

    public int? BlogCategoryId { get; set; }
    public BlogCategory? BlogCategory { get; set; }

    public bool IsPublished { get; set; } = true;
    public bool IsFeatured { get; set; }
    public int ViewCount { get; set; }
    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(200)] public string? MetaTitle { get; set; }
    [MaxLength(300)] public string? MetaDescription { get; set; }

    [NotMapped] public IEnumerable<string> TagList =>
        string.IsNullOrWhiteSpace(Tags) ? Array.Empty<string>()
        : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

public class SkillCategory
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [MaxLength(80)] public string IconClass { get; set; } = "fas fa-code";
    public int SortOrder { get; set; }
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}

public class Skill
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = "";
    [Range(0, 100)] public int Percentage { get; set; } = 80;
    public int SortOrder { get; set; }
    public int SkillCategoryId { get; set; }
    public SkillCategory? SkillCategory { get; set; }
}

public class Experience
{
    public int Id { get; set; }
    [Required, MaxLength(160)] public string Role { get; set; } = "";
    [Required, MaxLength(160)] public string Company { get; set; } = "";
    [MaxLength(160)] public string? Location { get; set; }
    [MaxLength(80)] public string? DateRange { get; set; }
    public bool IsCurrent { get; set; }
    /// <summary>One bullet per line.</summary>
    public string? Bullets { get; set; }
    [MaxLength(400)] public string? Tags { get; set; }
    public int SortOrder { get; set; }

    [NotMapped] public IEnumerable<string> BulletList =>
        string.IsNullOrWhiteSpace(Bullets) ? Array.Empty<string>()
        : Bullets.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    [NotMapped] public IEnumerable<string> TagList =>
        string.IsNullOrWhiteSpace(Tags) ? Array.Empty<string>()
        : Tags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

public class MediaAsset
{
    public int Id { get; set; }
    [Required, MaxLength(260)] public string FileName { get; set; } = "";
    [Required, MaxLength(260)] public string OriginalName { get; set; } = "";
    [Required, MaxLength(400)] public string Url { get; set; } = "";
    [MaxLength(120)] public string ContentType { get; set; } = "";
    [MaxLength(20)] public string Kind { get; set; } = "image";   // image | video | document
    public long SizeBytes { get; set; }
    [MaxLength(250)] public string? AltText { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

public class ContactMessage
{
    public int Id { get; set; }
    [Required, MaxLength(160)] public string Name { get; set; } = "";
    [Required, MaxLength(200), EmailAddress] public string Email { get; set; } = "";
    [MaxLength(250)] public string? Subject { get; set; }
    [Required, MaxLength(4000)] public string Message { get; set; } = "";
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AdminUser
{
    public int Id { get; set; }
    [Required, MaxLength(80)] public string Username { get; set; } = "";
    [Required, MaxLength(200)] public string Email { get; set; } = "";
    [Required, MaxLength(400)] public string PasswordHash { get; set; } = "";
    [Required, MaxLength(200)] public string PasswordSalt { get; set; } = "";
    [MaxLength(120)] public string DisplayName { get; set; } = "Administrator";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool MustChangePassword { get; set; } = true;
}
