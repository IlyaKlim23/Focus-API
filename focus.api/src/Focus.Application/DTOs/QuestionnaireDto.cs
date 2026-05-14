namespace Focus.Application.DTOs;

public record QuestionnaireDto(Guid Id, string Code, string Name, string Description);

public record QuestionnaireQuestionDto(Guid Id, string Text, int SortOrder, int MinValue, int MaxValue);

public record UserQuestionnaireScheduleDto(Guid QuestionnaireId, string Cadence, DateTime NextDueAtUtc, bool IsEnabled);

public record UpsertQuestionnaireScheduleRequest(Guid QuestionnaireId, string Cadence, DateTime? NextDueAtUtc, bool IsEnabled);

public record QuestionnaireAnswerItem(Guid QuestionId, int Value);

public record SubmitQuestionnaireRequest(Guid QuestionnaireId, IReadOnlyList<QuestionnaireAnswerItem> Answers);

public record QuestionnaireResponseDto(Guid Id, Guid QuestionnaireId, DateTime SubmittedAtUtc, int TotalScore);
