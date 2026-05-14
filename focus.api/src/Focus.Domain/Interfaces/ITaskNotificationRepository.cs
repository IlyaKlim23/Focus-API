using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

public interface ITaskNotificationRepository
{
    Task<bool> ExistsAsync(Guid taskId, DateTime slotStart, CancellationToken ct = default);
    Task AddAsync(TaskNotification notification, CancellationToken ct = default);
    Task UpdateAsync(TaskNotification notification, CancellationToken ct = default);
}
