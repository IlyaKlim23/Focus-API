using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

public interface IPsychologicalQuestionnaireRepository
{
    Task<IReadOnlyList<PsychologicalQuestionnaire>> GetActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<PsychologicalQuestionnaireQuestion>> GetQuestionsAsync(Guid questionnaireId, CancellationToken ct = default);
    Task<UserQuestionnaireSchedule?> GetUserScheduleAsync(Guid userId, Guid questionnaireId, CancellationToken ct = default);
    Task<IReadOnlyList<UserQuestionnaireSchedule>> GetUserSchedulesAsync(Guid userId, CancellationToken ct = default);
    Task<UserQuestionnaireSchedule> UpsertUserScheduleAsync(UserQuestionnaireSchedule schedule, CancellationToken ct = default);
    Task AddResponseAsync(QuestionnaireResponse response, IReadOnlyList<QuestionnaireResponseItem> items, CancellationToken ct = default);
    Task<IReadOnlyList<QuestionnaireResponse>> GetResponsesAsync(Guid userId, Guid questionnaireId, int take, CancellationToken ct = default);
}
