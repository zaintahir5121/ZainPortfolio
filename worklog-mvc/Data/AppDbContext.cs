using Microsoft.EntityFrameworkCore;
using WorkLogApp.Models;

namespace WorkLogApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User>          Users          { get; set; }
    public DbSet<Note>          Notes          { get; set; }
    public DbSet<ChecklistItem> ChecklistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Note>()
         .HasMany(n => n.ChecklistItems)
         .WithOne(c => c.Note)
         .HasForeignKey(c => c.NoteId)
         .OnDelete(DeleteBehavior.Cascade);
    }

    public static void Seed(AppDbContext db) { }
}
