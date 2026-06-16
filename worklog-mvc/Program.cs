using Microsoft.EntityFrameworkCore;
using WorkLogApp.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connStr = builder.Configuration.GetConnectionString("Default") ?? "";
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    if (connStr.StartsWith("Data Source=", StringComparison.OrdinalIgnoreCase))
        opt.UseSqlite(connStr);
    else
        opt.UseSqlServer(connStr);
});

builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", opt =>
    {
        opt.LoginPath         = "/Account/Login";
        opt.LogoutPath        = "/Account/Logout";
        opt.AccessDeniedPath  = "/Account/Login";
        opt.SlidingExpiration = true;
        opt.ExpireTimeSpan    = TimeSpan.FromDays(30);
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var ctx    = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    DatabaseInitializer.Initialize(ctx, logger);
}

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/Error");
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name:    "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
