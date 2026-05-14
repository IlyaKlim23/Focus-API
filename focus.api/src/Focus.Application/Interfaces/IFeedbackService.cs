using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateAsync(Guid userId, CreateFeedbackRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<FeedbackDto>> GetMineAsync(Guid userId, int take = 50, CancellationToken ct = default);
    Task<IReadOnlyList<FeedbackDto>> GetRecentAsync(int take = 200, CancellationToken ct = default);
}
