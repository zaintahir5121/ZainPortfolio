using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ZainPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    PasswordSalt = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MustChangePassword = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Role = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Company = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    DateRange = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    Bullets = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MediaAssets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    OriginalName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaAssets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PageViews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    PageType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ReferrerHost = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DeviceType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    VisitorId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IpHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    IsNewVisitor = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Day = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageViews", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Tagline = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    LogoText = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LogoImageUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    FaviconUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DarkBgColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    LightBgColor = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DefaultTheme = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HeadingFont = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    BodyFont = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    CornerRadius = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HeroBadge = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    HeroHeadingLine1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroHeadingHighlight = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroHeadingLine2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HeroSubheading = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TypewriterRoles = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    IntroVideoId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    IntroVideoAutoplay = table.Column<bool>(type: "bit", nullable: false),
                    StatYears = table.Column<int>(type: "int", nullable: false),
                    StatProjects = table.Column<int>(type: "int", nullable: false),
                    StatEnterprise = table.Column<int>(type: "int", nullable: false),
                    StatYearsLabel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StatProjectsLabel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    StatEnterpriseLabel = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    AboutTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AboutTitleHighlight = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AboutLead = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AboutBody = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProfileImageUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    AboutTags = table.Column<string>(type: "nvarchar(1200)", maxLength: 1200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    WhatsAppNumber = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: false),
                    Availability = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    GitHubUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    YouTubeUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    UpworkUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    CalendlyUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    TwitterUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    EnableBlog = table.Column<bool>(type: "bit", nullable: false),
                    EnableProjects = table.Column<bool>(type: "bit", nullable: false),
                    EnableChatbot = table.Column<bool>(type: "bit", nullable: false),
                    EnableWhatsAppFloat = table.Column<bool>(type: "bit", nullable: false),
                    EnableContactForm = table.Column<bool>(type: "bit", nullable: false),
                    EnableParticles = table.Column<bool>(type: "bit", nullable: false),
                    EnableNewsletter = table.Column<bool>(type: "bit", nullable: false),
                    EnableAnalytics = table.Column<bool>(type: "bit", nullable: false),
                    MetaTitle = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    MetaKeywords = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    GoogleAnalyticsId = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    GoogleSiteVerification = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    BingSiteVerification = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    OgImageUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    TwitterHandle = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    CanonicalBaseUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    GeoRegion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    GeoPlacename = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    GeoPosition = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    ContentLanguage = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ServiceArea = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    FooterText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscribers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    IsConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ConfirmToken = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    IpHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    SubscribedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UnsubscribedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(270)", maxLength: 270, nullable: false),
                    Excerpt = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    VideoId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    AuthorName = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    AuthorImageUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ReadMinutes = table.Column<int>(type: "int", nullable: false),
                    Tags = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    BlogCategoryId = table.Column<int>(type: "int", nullable: true),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MetaTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MetaDescription = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_BlogCategories_BlogCategoryId",
                        column: x => x.BlogCategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(220)", maxLength: 220, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Problem = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    Solution = table.Column<string>(type: "nvarchar(1500)", maxLength: 1500, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoverImageUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    IconClass = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: false),
                    GradientFrom = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    GradientTo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    VideoId = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    LiveUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    RepoUrl = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Stat1Icon = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Stat1Text = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    Stat2Icon = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: true),
                    Stat2Text = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    ProjectCategoryId = table.Column<int>(type: "int", nullable: true),
                    FilterKeys = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    ViewCount = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ProjectCategories_ProjectCategoryId",
                        column: x => x.ProjectCategoryId,
                        principalTable: "ProjectCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    SkillCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Skills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Skills_SkillCategories_SkillCategoryId",
                        column: x => x.SkillCategoryId,
                        principalTable: "SkillCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectImages_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdminUsers_Username",
                table: "AdminUsers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_Slug",
                table: "BlogCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_BlogCategoryId",
                table: "BlogPosts",
                column: "BlogCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsPublished_PublishedAt",
                table: "BlogPosts",
                columns: new[] { "IsPublished", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_Day",
                table: "PageViews",
                column: "Day");

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_PageType_EntityId",
                table: "PageViews",
                columns: new[] { "PageType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_PageViews_VisitorId",
                table: "PageViews",
                column: "VisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectCategories_Slug",
                table: "ProjectCategories",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectImages_ProjectId",
                table: "ProjectImages",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_IsPublished_SortOrder",
                table: "Projects",
                columns: new[] { "IsPublished", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCategoryId",
                table: "Projects",
                column: "ProjectCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Slug",
                table: "Projects",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Skills_SkillCategoryId",
                table: "Skills",
                column: "SkillCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_Email",
                table: "Subscribers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscribers_SubscribedAt",
                table: "Subscribers",
                column: "SubscribedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminUsers");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "ContactMessages");

            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.DropTable(
                name: "MediaAssets");

            migrationBuilder.DropTable(
                name: "PageViews");

            migrationBuilder.DropTable(
                name: "ProjectImages");

            migrationBuilder.DropTable(
                name: "SiteSettings");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Subscribers");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "SkillCategories");

            migrationBuilder.DropTable(
                name: "ProjectCategories");
        }
    }
}
