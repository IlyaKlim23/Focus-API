using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

public interface IUserFeedbackRepository
{
    Task<UserFeedback> AddAsync(UserFeedback feedback, CancellationToken ct = default);
    Task<IReadOnlyList<UserFeedback>> GetByUserAsync(Guid userId, int take = 100, CancellationToken ct = default);
    Task<IReadOnlyList<UserFeedback>> GetRecentAsync(int take = 200, CancellationToken ct = default);
}
