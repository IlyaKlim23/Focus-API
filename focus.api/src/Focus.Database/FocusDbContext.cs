using Focus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Focus.Database;

public class FocusDbContext(DbContextOptions<FocusDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
    public DbSet<DailyNote> DailyNotes => Set<DailyNote>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.DisplayName).HasMaxLength(256);
            e.Property(x => x.PasswordHash).HasMaxLength(256);
        });

        modelBuilder.Entity<TaskCategory>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne<User>().WithMany().HasForeignKey(x => x.UserId);
            e.Property(x => x.Name).HasMaxLength(256);
            e.Property(x => x.Color).HasMaxLength(32);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.HasOne(x => x.Category).WithMany().HasForeignKey(x => x.CategoryId).IsRequired(false);
            e.Property(x => x.Title).HasMaxLength(512);
        });

        modelBuilder.Entity<DailyNote>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.Date }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
            e.Property(x => x.ExtractedFactors).HasMaxLength(1024);
        });
    }
}
