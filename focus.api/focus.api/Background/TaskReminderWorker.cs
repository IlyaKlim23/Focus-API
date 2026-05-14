using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Focus.Api.Background;

public class TaskReminderWorker(IServiceScopeFactory scopeFactory, ILogger<TaskReminderWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Нормальное завершение фонового сервиса при остановке приложения
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Task reminder worker failed");
            }

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Токен отмены сработал во время ожидания
                break;
            }
        }
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var taskRepo = scope.ServiceProvider.GetRequiredService<ITaskRepository>();
        var notifRepo = scope.ServiceProvider.GetRequiredService<ITaskNotificationRepository>();
        var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

        var db = scope.ServiceProvider.GetRequiredService<Focus.Database.FocusDbContext>();
        var prefs = db.NotificationPreferences.Where(x => x.IsEnabled).ToList();
        var now = DateTime.UtcNow;

        foreach (var pref in prefs)
        {
            var dueFrom = now.AddMinutes(pref.RemindBeforeMinutes);
            var dueTo = dueFrom.AddMinutes(1);
            var tasks = await taskRepo.GetPendingDueBetweenAsync(pref.UserId, dueFrom, dueTo, ct);
            foreach (var task in tasks)
            {
                var slotStart = task.DueDate ?? dueFrom;
                if (await notifRepo.ExistsAsync(task.Id, slotStart, ct))
                    continue;

                var record = new TaskNotification
                {
                    Id = Guid.NewGuid(),
                    UserId = pref.UserId,
                    TaskId = task.Id,
                    SlotStart = slotStart,
                    ScheduledForUtc = slotStart.AddMinutes(-pref.RemindBeforeMinutes),
                    Status = "Pending",
                    AttemptCount = 0,
                    CreatedAt = now
                };

                try
                {
                    var dueLocal = slotStart.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
                    await emailSender.SendAsync(
                        pref.Email,
                        $"Напоминание о задаче: {task.Title}",
                        $"Через {pref.RemindBeforeMinutes} минут начинается задача \"{task.Title}\".\nСрок/начало: {dueLocal}",
                        ct);
                    record.Status = "Sent";
                    record.SentAtUtc = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    record.Status = "Failed";
                    record.LastError = ex.Message;
                    logger.LogWarning(ex, "Failed to send reminder for task {TaskId}", task.Id);
                }

                record.AttemptCount += 1;
                await notifRepo.AddAsync(record, ct);
            }
        }
    }
}
