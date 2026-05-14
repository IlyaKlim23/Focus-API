using Focus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Focus.Database;

public class FocusDbContext(DbContextOptions<FocusDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<TaskCategory> TaskCategories => Set<TaskCategory>();
    public DbSet<DailyNote> DailyNotes => Set<DailyNote>();
    public DbSet<ProductivityPrediction> ProductivityPredictions => Set<ProductivityPrediction>();
    public DbSet<ScheduledSlot> ScheduledSlots => Set<ScheduledSlot>();
    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();
    public DbSet<TaskNotification> TaskNotifications => Set<TaskNotification>();
    public DbSet<PsychologicalQuestionnaire> PsychologicalQuestionnaires => Set<PsychologicalQuestionnaire>();
    public DbSet<PsychologicalQuestionnaireQuestion> PsychologicalQuestionnaireQuestions => Set<PsychologicalQuestionnaireQuestion>();
    public DbSet<UserQuestionnaireSchedule> UserQuestionnaireSchedules => Set<UserQuestionnaireSchedule>();
    public DbSet<QuestionnaireResponse> QuestionnaireResponses => Set<QuestionnaireResponse>();
    public DbSet<QuestionnaireResponseItem> QuestionnaireResponseItems => Set<QuestionnaireResponseItem>();
    public DbSet<UserFeedback> UserFeedbacks => Set<UserFeedback>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.DisplayName).HasMaxLength(256);
            e.Property(x => x.PasswordHash).HasMaxLength(256);
            e.Property(x => x.Role).HasMaxLength(32).HasDefaultValue("User");
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

        modelBuilder.Entity<ProductivityPrediction>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.SlotStart }).IsUnique();
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.Score).HasPrecision(9, 6);
            e.Property(x => x.Factors).HasMaxLength(4096);
        });

        modelBuilder.Entity<ScheduledSlot>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId);
            e.HasIndex(x => new { x.UserId, x.SlotStart });
            e.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Task).WithMany().HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NotificationPreference>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.UserId).IsUnique();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.UnavailableFromMinutes);
            e.Property(x => x.UnavailableToMinutes);
        });

        modelBuilder.Entity<TaskNotification>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.Status, x.ScheduledForUtc });
            e.HasIndex(x => new { x.TaskId, x.SlotStart }).IsUnique();
            e.Property(x => x.Status).HasMaxLength(32);
            e.Property(x => x.LastError).HasMaxLength(1024);
        });

        modelBuilder.Entity<PsychologicalQuestionnaire>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.Code).HasMaxLength(64);
            e.Property(x => x.Name).HasMaxLength(256);
            e.Property(x => x.Description).HasMaxLength(2048);
        });

        modelBuilder.Entity<PsychologicalQuestionnaireQuestion>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.QuestionnaireId, x.SortOrder });
            e.Property(x => x.Text).HasMaxLength(1024);
        });

        modelBuilder.Entity<UserQuestionnaireSchedule>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.QuestionnaireId }).IsUnique();
            e.HasIndex(x => new { x.IsEnabled, x.NextDueAtUtc });
            e.Property(x => x.Cadence).HasMaxLength(32);
        });

        modelBuilder.Entity<QuestionnaireResponse>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.SubmittedAtUtc });
        });

        modelBuilder.Entity<QuestionnaireResponseItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.ResponseId, x.QuestionId }).IsUnique();
        });

        modelBuilder.Entity<UserFeedback>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.UserId, x.CreatedAt });
            e.Property(x => x.Message).HasMaxLength(2048);
        });
    }
}
