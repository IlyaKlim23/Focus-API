using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Focus.Domain.ValueObjects;

namespace Focus.Infrastructure.Repositories;

public class EfTaskRepository(FocusDbContext db) : ITaskRepository
{
    public async Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default) =>
        await db.Tasks.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, ct);

    public async Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default)
    {
        var query = db.Tasks.Where(x => x.UserId == userId);
        if (from.HasValue) query = query.Where(x => x.CreatedAt >= from.Value);
        if (to.HasValue) query = query.Where(x => x.CreatedAt <= to.Value);
        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<TaskItem>> GetPendingDueBetweenAsync(
        Guid userId,
        DateTime dueFromUtc,
        DateTime dueToUtc,
        CancellationToken ct = default) =>
        await db.Tasks
            .Where(x => x.UserId == userId &&
                        x.DueDate != null &&
                        x.DueDate >= dueFromUtc &&
                        x.DueDate <= dueToUtc &&
                        x.Status != TaskItemStatus.Done &&
                        x.Status != TaskItemStatus.Cancelled)
            .OrderBy(x => x.DueDate)
            .ToListAsync(ct);

    public async Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default)
    {
        db.Tasks.Add(task);
        await db.SaveChangesAsync(ct);
        return task;
    }

    public async Task UpdateAsync(TaskItem task, CancellationToken ct = default)
    {
        db.Tasks.Update(task);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        await db.Tasks.Where(x => x.Id == id && x.UserId == userId).ExecuteDeleteAsync(ct);
    }
}
