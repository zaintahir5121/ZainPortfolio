namespace ZainPortfolio.Models.ViewModels;

public class PageMeta
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Keywords { get; set; }
    public string? CanonicalPath { get; set; }
    public string? ImageUrl { get; set; }
    public string OgType { get; set; } = "website";
    public DateTime? PublishedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
    public string? ArticleSection { get; set; }
    public IEnumerable<string> ArticleTags { get; set; } = Array.Empty<string>();
    /// <summary>Extra JSON-LD blocks rendered into the head.</summary>
    public List<string> JsonLd { get; set; } = new();
    public List<(string Name, string Url)> Breadcrumbs { get; set; } = new();
}

public abstract class PublicViewModel
{
    public SiteSetting Settings { get; set; } = new();
    public PageMeta Meta { get; set; } = new();
}

public class HomeViewModel : PublicViewModel
{
    public List<Project> FeaturedProjects { get; set; } = new();
    public List<ProjectCategory> ProjectCategories { get; set; } = new();
    public List<SkillCategory> SkillCategories { get; set; } = new();
    public List<Experience> Experiences { get; set; } = new();
    public List<BlogPost> LatestPosts { get; set; } = new();
    public int TotalProjects { get; set; }
}

public class ProjectListViewModel : PublicViewModel
{
    public List<Project> Projects { get; set; } = new();
    public List<ProjectCategory> Categories { get; set; } = new();
    public string CurrentCategory { get; set; } = "all";
    public string? Query { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }
}

public class ProjectDetailViewModel : PublicViewModel
{
    public Project Project { get; set; } = new();
    public List<Project> Related { get; set; } = new();
}

public class BlogListViewModel : PublicViewModel
{
    public List<BlogPost> Posts { get; set; } = new();
    public BlogPost? Featured { get; set; }
    public List<BlogCategory> Categories { get; set; } = new();
    public List<string> PopularTags { get; set; } = new();
    public string? CurrentCategory { get; set; }
    public string? CurrentTag { get; set; }
    public string? Query { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int TotalCount { get; set; }
}

public class BlogDetailViewModel : PublicViewModel
{
    public BlogPost Post { get; set; } = new();
    public List<BlogPost> Related { get; set; } = new();
    public BlogPost? Previous { get; set; }
    public BlogPost? Next { get; set; }
}
