using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<NotificationPreference> UpsertAsync(NotificationPreference preference, CancellationToken ct = default);
}
