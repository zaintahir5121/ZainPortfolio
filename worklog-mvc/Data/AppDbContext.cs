using Microsoft.EntityFrameworkCore;
using WorkLogApp.Models;

namespace WorkLogApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User>           Users            { get; set; }
    public DbSet<LogEntry>       LogEntries       { get; set; }
    public DbSet<RecurringEntry> RecurringEntries { get; set; }
    public DbSet<Achievement>    Achievements     { get; set; }
    public DbSet<Experience>     Experiences      { get; set; }
    public DbSet<LearningItem>   LearningItems    { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<LogEntry>()
         .Property(l => l.Hours)
         .HasColumnType("decimal(5,2)");

        b.Entity<RecurringEntry>()
         .Property(r => r.Hours)
         .HasColumnType("decimal(5,2)");
    }

    public static void Seed(AppDbContext db)
    {
        if (db.Users.Any()) return;

        db.Users.AddRange(
            new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Name = "Admin",     Role = "admin" },
            new User { Username = "john",  PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),  Name = "John Doe",  Role = "employee" },
            new User { Username = "sara",  PasswordHash = BCrypt.Net.BCrypt.HashPassword("pass123"),  Name = "Sara Khan", Role = "employee" }
        );
        db.SaveChanges();
    }
}
