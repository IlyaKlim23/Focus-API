using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfNotificationPreferenceRepository(FocusDbContext db) : INotificationPreferenceRepository
{
    public Task<NotificationPreference?> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        db.NotificationPreferences.FirstOrDefaultAsync(x => x.UserId == userId, ct);

    public async Task<NotificationPreference> UpsertAsync(NotificationPreference preference, CancellationToken ct = default)
    {
        var existing = await db.NotificationPreferences.FirstOrDefaultAsync(x => x.UserId == preference.UserId, ct);
        if (existing == null)
        {
            db.NotificationPreferences.Add(preference);
        }
        else
        {
            existing.Email = preference.Email;
            existing.IsEnabled = preference.IsEnabled;
            existing.RemindBeforeMinutes = preference.RemindBeforeMinutes;
            existing.UnavailableFromMinutes = preference.UnavailableFromMinutes;
            existing.UnavailableToMinutes = preference.UnavailableToMinutes;
            existing.UpdatedAt = preference.UpdatedAt;
        }

        await db.SaveChangesAsync(ct);
        return existing ?? preference;
    }
}
