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
    public DbSet<FeedbackEntry>  Feedbacks        { get; set; }

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
        // No default users — users register their own accounts.
    }
}
