using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

public interface INotificationPreferenceService
{
    Task<NotificationPreferenceDto?> GetAsync(Guid userId, CancellationToken ct = default);
    Task<NotificationPreferenceDto> UpsertAsync(Guid userId, UpsertNotificationPreferenceRequest request, CancellationToken ct = default);
}
