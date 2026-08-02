using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Models;

namespace ZainPortfolio.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SiteSetting> SiteSettings => Set<SiteSetting>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectCategory> ProjectCategories => Set<ProjectCategory>();
    public DbSet<ProjectImage> ProjectImages => Set<ProjectImage>();
    public DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
    public DbSet<SkillCategory> SkillCategories => Set<SkillCategory>();
    public DbSet<Skill> Skills => Set<Skill>();
    public DbSet<Experience> Experiences => Set<Experience>();
    public DbSet<MediaAsset> MediaAssets => Set<MediaAsset>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<Subscriber> Subscribers => Set<Subscriber>();
    public DbSet<PageView> PageViews => Set<PageView>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<Project>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => new { x.IsPublished, x.SortOrder });
            e.HasOne(x => x.ProjectCategory)
             .WithMany(c => c.Projects)
             .HasForeignKey(x => x.ProjectCategoryId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<ProjectImage>()
            .HasOne(x => x.Project)
            .WithMany(p => p.Images)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<BlogPost>(e =>
        {
            e.HasIndex(x => x.Slug).IsUnique();
            e.HasIndex(x => new { x.IsPublished, x.PublishedAt });
            e.HasOne(x => x.BlogCategory)
             .WithMany(c => c.Posts)
             .HasForeignKey(x => x.BlogCategoryId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        b.Entity<ProjectCategory>().HasIndex(x => x.Slug).IsUnique();
        b.Entity<BlogCategory>().HasIndex(x => x.Slug).IsUnique();
        b.Entity<AdminUser>().HasIndex(x => x.Username).IsUnique();

        b.Entity<Skill>()
            .HasOne(x => x.SkillCategory)
            .WithMany(c => c.Skills)
            .HasForeignKey(x => x.SkillCategoryId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<Subscriber>(e =>
        {
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.SubscribedAt);
        });

        b.Entity<PageView>(e =>
        {
            e.HasIndex(x => x.Day);
            e.HasIndex(x => new { x.PageType, x.EntityId });
            e.HasIndex(x => x.VisitorId);
        });
    }
}
