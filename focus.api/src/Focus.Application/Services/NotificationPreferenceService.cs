using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class NotificationPreferenceService(INotificationPreferenceRepository repository) : INotificationPreferenceService
{
    public async Task<NotificationPreferenceDto?> GetAsync(Guid userId, CancellationToken ct = default)
    {
        var entity = await repository.GetByUserIdAsync(userId, ct);
        return entity == null ? null : Map(entity);
    }

    public async Task<NotificationPreferenceDto> UpsertAsync(Guid userId, UpsertNotificationPreferenceRequest request, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var entity = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = request.Email.Trim(),
            IsEnabled = request.IsEnabled,
            RemindBeforeMinutes = Math.Clamp(request.RemindBeforeMinutes, 5, 1440),
            UnavailableFromMinutes = NormalizeMinute(request.UnavailableFromMinutes),
            UnavailableToMinutes = NormalizeMinute(request.UnavailableToMinutes),
            CreatedAt = now,
            UpdatedAt = now
        };
        var saved = await repository.UpsertAsync(entity, ct);
        return Map(saved);
    }

    private static NotificationPreferenceDto Map(NotificationPreference p) =>
        new(p.Email, p.IsEnabled, p.RemindBeforeMinutes, p.UnavailableFromMinutes, p.UnavailableToMinutes);

    private static int? NormalizeMinute(int? minute)
    {
        if (!minute.HasValue) return null;
        return Math.Clamp(minute.Value, 0, 1439);
    }
}
