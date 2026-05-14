using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

public interface IDeveloperAnalyticsService
{
    Task<DeveloperQuestionnaireAnalyticsResponse> GetQuestionnaireAnalyticsAsync(int days = 30, CancellationToken ct = default);
}
