using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", opt =>
    {
        opt.LoginPath        = "/Account/Login";
        opt.LogoutPath       = "/Account/Logout";
        opt.AccessDeniedPath = "/Account/Login";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan   = TimeSpan.FromDays(7);
    });

builder.Services.AddHttpClient<IOllamaService, OllamaService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Ollama:Url"] ?? "http://localhost:11434");
    client.Timeout     = TimeSpan.FromSeconds(90);
});

var app = builder.Build();

// Auto-create schema + seed on first run
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    ctx.Database.EnsureCreated();
    AppDbContext.Seed(ctx);

    ctx.Database.ExecuteSqlRaw("""
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RecurringEntries')
        CREATE TABLE RecurringEntries (
            Id            INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_RecurringEntries PRIMARY KEY,
            UserId        INT            NOT NULL,
            Project       NVARCHAR(200)  NOT NULL DEFAULT N'',
            Description   NVARCHAR(1000) NOT NULL DEFAULT N'',
            Hours         DECIMAL(5,2)   NOT NULL DEFAULT 0,
            Tags          NVARCHAR(200)  NOT NULL DEFAULT N'',
            Schedule      NVARCHAR(100)  NOT NULL DEFAULT N'weekdays',
            IsActive      BIT            NOT NULL DEFAULT 1,
            LastFiredDate DATE           NULL,
            CreatedAt     DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_RecurringEntries_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
        )
    """);
    ctx.Database.ExecuteSqlRaw("""
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Achievements')
        CREATE TABLE Achievements (
            Id          INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Achievements PRIMARY KEY,
            UserId      INT            NOT NULL,
            Title       NVARCHAR(300)  NOT NULL DEFAULT N'',
            Description NVARCHAR(2000) NOT NULL DEFAULT N'',
            Date        DATE           NOT NULL DEFAULT GETDATE(),
            Category    NVARCHAR(50)   NOT NULL DEFAULT N'milestone',
            CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_Achievements_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
        )
    """);
    ctx.Database.ExecuteSqlRaw("""
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Experiences')
        CREATE TABLE Experiences (
            Id          INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_Experiences PRIMARY KEY,
            UserId      INT            NOT NULL,
            Company     NVARCHAR(200)  NOT NULL DEFAULT N'',
            Role        NVARCHAR(200)  NOT NULL DEFAULT N'',
            Location    NVARCHAR(200)  NOT NULL DEFAULT N'',
            StartDate   DATE           NOT NULL DEFAULT GETDATE(),
            EndDate     DATE           NULL,
            IsCurrent   BIT            NOT NULL DEFAULT 0,
            Description NVARCHAR(2000) NOT NULL DEFAULT N'',
            Tags        NVARCHAR(500)  NOT NULL DEFAULT N'',
            CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_Experiences_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
        )
    """);
    ctx.Database.ExecuteSqlRaw("""
        IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LearningItems')
        CREATE TABLE LearningItems (
            Id            INT            NOT NULL IDENTITY(1,1) CONSTRAINT PK_LearningItems PRIMARY KEY,
            UserId        INT            NOT NULL,
            Title         NVARCHAR(300)  NOT NULL DEFAULT N'',
            Type          NVARCHAR(50)   NOT NULL DEFAULT N'course',
            Source        NVARCHAR(200)  NOT NULL DEFAULT N'',
            Status        NVARCHAR(50)   NOT NULL DEFAULT N'in-progress',
            Notes         NVARCHAR(2000) NOT NULL DEFAULT N'',
            StartedDate   DATE           NULL,
            CompletedDate DATE           NULL,
            Rating        INT            NOT NULL DEFAULT 0,
            CreatedAt     DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
            CONSTRAINT FK_LearningItems_Users FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
        )
    """);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
