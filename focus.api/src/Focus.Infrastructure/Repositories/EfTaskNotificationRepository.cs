using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfTaskNotificationRepository(FocusDbContext db) : ITaskNotificationRepository
{
    public Task<bool> ExistsAsync(Guid taskId, DateTime slotStart, CancellationToken ct = default) =>
        db.TaskNotifications.AnyAsync(x => x.TaskId == taskId && x.SlotStart == slotStart && x.Status == "Sent", ct);

    public async Task AddAsync(TaskNotification notification, CancellationToken ct = default)
    {
        db.TaskNotifications.Add(notification);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(TaskNotification notification, CancellationToken ct = default)
    {
        db.TaskNotifications.Update(notification);
        await db.SaveChangesAsync(ct);
    }
}
