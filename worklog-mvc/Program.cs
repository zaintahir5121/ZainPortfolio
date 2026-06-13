using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;
using WorkLogApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// ── Database ──────────────────────────────────────────────────────────────────
var connStr = builder.Configuration.GetConnectionString("Default") ?? "";
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    if (connStr.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        opt.UseSqlite(connStr);
    else
        opt.UseSqlServer(connStr);
});

// ── Authentication (cookie-based, no ASP.NET Identity) ────────────────────────
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", opt =>
    {
        opt.LoginPath         = "/Account/Login";
        opt.LogoutPath        = "/Account/Logout";
        opt.AccessDeniedPath  = "/Account/Login";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan    = TimeSpan.FromDays(7);
    });

// ── Ollama (local AI, runs on http://localhost:11434) ─────────────────────────
builder.Services.AddHttpClient<IOllamaService, OllamaService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Ollama:Url"] ?? "http://localhost:11434");
    client.Timeout     = TimeSpan.FromSeconds(90);
});

var app = builder.Build();

// ── Auto-create / auto-migrate database on startup ────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var ctx    = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    DatabaseInitializer.Initialize(ctx, logger);
}

// ── Middleware pipeline ───────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/NotFound");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
