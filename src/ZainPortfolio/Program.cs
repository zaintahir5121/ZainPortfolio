using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ZainPortfolio.Data;
using ZainPortfolio.Services;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------- database
// SQL Server is the production/IIS target. Sqlite is offered so the site can be
// run locally (or in CI) without a SQL Server instance — switch with Database:Provider.
var provider = builder.Configuration["Database:Provider"] ?? "SqlServer";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Server=(localdb)\\MSSQLLocalDB;Database=ZainPortfolio;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (provider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
        options.UseSqlite(connectionString, x => x.MigrationsAssembly("ZainPortfolio"));
    else
        options.UseSqlServer(connectionString, x =>
        {
            x.MigrationsAssembly("ZainPortfolio");
            x.EnableRetryOnFailure(3);
        });
});

// ---------------------------------------------------------------- services
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ISiteSettingsService, SiteSettingsService>();
builder.Services.AddSingleton<IHtmlSanitizerService, HtmlSanitizerService>();
builder.Services.AddSingleton<ISeoService, SeoService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddResponseCompression();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/account/login";
        options.LogoutPath = "/admin/account/logout";
        options.AccessDeniedPath = "/admin/account/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.Name = "zp.auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminOnly", p => p.RequireAuthenticatedUser()));

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 64L * 1024 * 1024;   // matches MediaService limit
});

var app = builder.Build();

// ---------------------------------------------------------------- pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseStatusCodePagesWithReExecute("/error/{0}");
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// ---------------------------------------------------------------- startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        await DbSeeder.SeedAsync(db, app.Configuration, logger);
        logger.LogInformation("Database ready ({Provider}).", provider);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialisation failed. The site will start but content will be unavailable.");
    }
}

app.Run();
