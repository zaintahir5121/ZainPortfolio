using Microsoft.EntityFrameworkCore;
using WorkLogApp.Models;

namespace WorkLogApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User>          Users          { get; set; }
    public DbSet<Note>          Notes          { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }
    public DbSet<WorkEntry>     WorkEntries    { get; set; }
    public DbSet<WorkTask>      WorkTasks      { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Note>()
         .HasMany(n => n.ChecklistItems)
         .WithOne(c => c.Note)
         .HasForeignKey(c => c.NoteId)
         .OnDelete(DeleteBehavior.Cascade);

        b.Entity<WorkEntry>()
         .HasMany(e => e.Tasks)
         .WithOne(t => t.WorkEntry)
         .HasForeignKey(t => t.WorkEntryId)
         .OnDelete(DeleteBehavior.Cascade);
    }

    public static void Seed(AppDbContext db) { }
}
