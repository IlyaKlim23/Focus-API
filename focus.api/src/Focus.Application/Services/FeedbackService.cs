using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class FeedbackService(IUserFeedbackRepository repository) : IFeedbackService
{
    public async Task<FeedbackDto> CreateAsync(Guid userId, CreateFeedbackRequest request, CancellationToken ct = default)
    {
        var entity = new UserFeedback
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Message = request.Message.Trim(),
            Rating = Math.Clamp(request.Rating, 1, 5),
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddAsync(entity, ct);
        return Map(entity);
    }

    public async Task<IReadOnlyList<FeedbackDto>> GetMineAsync(Guid userId, int take = 50, CancellationToken ct = default) =>
        (await repository.GetByUserAsync(userId, take, ct)).Select(Map).ToList();

    public async Task<IReadOnlyList<FeedbackDto>> GetRecentAsync(int take = 200, CancellationToken ct = default) =>
        (await repository.GetRecentAsync(take, ct)).Select(Map).ToList();

    private static FeedbackDto Map(UserFeedback x) => new(x.Id, x.UserId, x.Message, x.Rating, x.CreatedAt);
}
