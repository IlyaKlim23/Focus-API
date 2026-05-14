using Focus.Database;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Focus.Infrastructure.Repositories;

public class EfUserFeedbackRepository(FocusDbContext db) : IUserFeedbackRepository
{
    public async Task<UserFeedback> AddAsync(UserFeedback feedback, CancellationToken ct = default)
    {
        db.UserFeedbacks.Add(feedback);
        await db.SaveChangesAsync(ct);
        return feedback;
    }

    public async Task<IReadOnlyList<UserFeedback>> GetByUserAsync(Guid userId, int take = 100, CancellationToken ct = default) =>
        await db.UserFeedbacks
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Take(Math.Clamp(take, 1, 200))
            .ToListAsync(ct);

    public async Task<IReadOnlyList<UserFeedback>> GetRecentAsync(int take = 200, CancellationToken ct = default) =>
        await db.UserFeedbacks
            .OrderByDescending(x => x.CreatedAt)
            .Take(Math.Clamp(take, 1, 500))
            .ToListAsync(ct);
}
