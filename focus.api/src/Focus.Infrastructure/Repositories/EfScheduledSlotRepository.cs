using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfScheduledSlotRepository(FocusDbContext db) : IScheduledSlotRepository
{
    public async Task<IReadOnlyList<ScheduledSlot>> GetByUserAndDayAsync(Guid userId, DateTime dayStartUtc, DateTime dayEndUtc, CancellationToken ct = default) =>
        await db.ScheduledSlots
            .Where(x => x.UserId == userId && x.SlotStart >= dayStartUtc && x.SlotStart < dayEndUtc)
            .OrderBy(x => x.SlotStart)
            .ToListAsync(ct);

    public async Task ReplaceDayAsync(Guid userId, DateTime dayStartUtc, DateTime dayEndUtc, IReadOnlyList<ScheduledSlot> slots, CancellationToken ct = default)
    {
        await db.ScheduledSlots
            .Where(x => x.UserId == userId && x.SlotStart >= dayStartUtc && x.SlotStart < dayEndUtc)
            .ExecuteDeleteAsync(ct);
        db.ScheduledSlots.AddRange(slots);
        await db.SaveChangesAsync(ct);
    }

    public async Task<ScheduledSlot> AddAsync(ScheduledSlot slot, CancellationToken ct = default)
    {
        db.ScheduledSlots.Add(slot);
        await db.SaveChangesAsync(ct);
        return slot;
    }

    public Task<ScheduledSlot?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        db.ScheduledSlots.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

    public async Task UpdateAsync(ScheduledSlot slot, CancellationToken ct = default)
    {
        db.ScheduledSlots.Update(slot);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        await db.ScheduledSlots.Where(x => x.Id == id && x.UserId == userId).ExecuteDeleteAsync(ct);
    }
}
