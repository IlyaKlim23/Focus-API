using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

public interface IScheduledSlotRepository
{
    Task<IReadOnlyList<ScheduledSlot>> GetByUserAndDayAsync(Guid userId, DateTime dayStartUtc, DateTime dayEndUtc, CancellationToken ct = default);
    Task ReplaceDayAsync(Guid userId, DateTime dayStartUtc, DateTime dayEndUtc, IReadOnlyList<ScheduledSlot> slots, CancellationToken ct = default);
    Task<ScheduledSlot> AddAsync(ScheduledSlot slot, CancellationToken ct = default);
    Task<ScheduledSlot?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);
    Task UpdateAsync(ScheduledSlot slot, CancellationToken ct = default);
    Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}
