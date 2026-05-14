using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class PsychologicalQuestionnaireService(
    IPsychologicalQuestionnaireRepository repository,
    ITelemetryWriter? telemetry = null) : IPsychologicalQuestionnaireService
{
    private readonly ITelemetryWriter? _telemetry = telemetry;
    public async Task<IReadOnlyList<QuestionnaireDto>> GetActiveAsync(CancellationToken ct = default) =>
        (await repository.GetActiveAsync(ct))
        .Select(x => new QuestionnaireDto(x.Id, x.Code, x.Name, x.Description))
        .ToList();

    public async Task<IReadOnlyList<QuestionnaireQuestionDto>> GetQuestionsAsync(Guid questionnaireId, CancellationToken ct = default) =>
        (await repository.GetQuestionsAsync(questionnaireId, ct))
        .Select(x => new QuestionnaireQuestionDto(x.Id, x.Text, x.SortOrder, x.MinValue, x.MaxValue))
        .ToList();

    public async Task<IReadOnlyList<UserQuestionnaireScheduleDto>> GetUserSchedulesAsync(Guid userId, CancellationToken ct = default) =>
        (await repository.GetUserSchedulesAsync(userId, ct))
        .Select(x => new UserQuestionnaireScheduleDto(x.QuestionnaireId, x.Cadence, x.NextDueAtUtc, x.IsEnabled))
        .ToList();

    public async Task<UserQuestionnaireScheduleDto> UpsertScheduleAsync(Guid userId, UpsertQuestionnaireScheduleRequest request, CancellationToken ct = default)
    {
        var cadence = request.Cadence.Equals("daily", StringComparison.OrdinalIgnoreCase) ? "Daily" : "Weekly";
        var now = DateTime.UtcNow;
        var nextDue = request.NextDueAtUtc ?? (cadence == "Daily" ? now.Date.AddDays(1) : now.Date.AddDays(7));

        var saved = await repository.UpsertUserScheduleAsync(new UserQuestionnaireSchedule
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            QuestionnaireId = request.QuestionnaireId,
            Cadence = cadence,
            NextDueAtUtc = DateTime.SpecifyKind(nextDue, DateTimeKind.Utc),
            IsEnabled = request.IsEnabled,
            CreatedAt = now,
            UpdatedAt = now
        }, ct);

        return new UserQuestionnaireScheduleDto(saved.QuestionnaireId, saved.Cadence, saved.NextDueAtUtc, saved.IsEnabled);
    }

    public async Task<QuestionnaireResponseDto> SubmitAsync(Guid userId, SubmitQuestionnaireRequest request, CancellationToken ct = default)
    {
        var questions = await repository.GetQuestionsAsync(request.QuestionnaireId, ct);
        var questionMap = questions.ToDictionary(x => x.Id);
        var total = 0;
        var responseId = Guid.NewGuid();
        var items = new List<QuestionnaireResponseItem>();

        foreach (var answer in request.Answers)
        {
            if (!questionMap.TryGetValue(answer.QuestionId, out var question))
                continue;
            var normalizedValue = Math.Clamp(answer.Value, question.MinValue, question.MaxValue);
            total += normalizedValue;
            items.Add(new QuestionnaireResponseItem
            {
                Id = Guid.NewGuid(),
                ResponseId = responseId,
                QuestionId = answer.QuestionId,
                Value = normalizedValue
            });
        }

        var response = new QuestionnaireResponse
        {
            Id = responseId,
            UserId = userId,
            QuestionnaireId = request.QuestionnaireId,
            SubmittedAtUtc = DateTime.UtcNow,
            TotalScore = total,
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddResponseAsync(response, items, ct);
        // Пишем агрегат опросника в ClickHouse для developer-аналитики
        // В обычном потоке не должно падать при проблемах телеметрии
        _telemetry?.TrackQuestionnaireIndicator(userId, request.QuestionnaireId, response.TotalScore, items.Count);

        return new QuestionnaireResponseDto(response.Id, response.QuestionnaireId, response.SubmittedAtUtc, response.TotalScore);
    }

    public async Task<IReadOnlyList<QuestionnaireResponseDto>> GetRecentResponsesAsync(Guid userId, Guid questionnaireId, int take, CancellationToken ct = default) =>
        (await repository.GetResponsesAsync(userId, questionnaireId, Math.Clamp(take, 1, 100), ct))
        .Select(x => new QuestionnaireResponseDto(x.Id, x.QuestionnaireId, x.SubmittedAtUtc, x.TotalScore))
        .ToList();
}
