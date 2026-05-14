using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

public interface IPsychologicalQuestionnaireService
{
    Task<IReadOnlyList<QuestionnaireDto>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<QuestionnaireQuestionDto>> GetQuestionsAsync(Guid questionnaireId, CancellationToken ct = default);
    Task<IReadOnlyList<UserQuestionnaireScheduleDto>> GetUserSchedulesAsync(Guid userId, CancellationToken ct = default);
    Task<UserQuestionnaireScheduleDto> UpsertScheduleAsync(Guid userId, UpsertQuestionnaireScheduleRequest request, CancellationToken ct = default);
    Task<QuestionnaireResponseDto> SubmitAsync(Guid userId, SubmitQuestionnaireRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<QuestionnaireResponseDto>> GetRecentResponsesAsync(Guid userId, Guid questionnaireId, int take, CancellationToken ct = default);
}
