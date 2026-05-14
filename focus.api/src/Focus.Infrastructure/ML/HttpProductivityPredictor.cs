using System.Net.Http.Json;
using Focus.Domain.Entities;
using Microsoft.Extensions.Http;
using Focus.Domain.Interfaces;
using Focus.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Focus.Infrastructure.ML;

public class HttpProductivityPredictor(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MlServiceOptions> optionsMonitor,
    ITaskRepository taskRepository,
    IPsychologicalQuestionnaireRepository questionnaireRepository,
    ILogger<HttpProductivityPredictor> logger) : IProductivityPredictor
{
    private const string ClientName = "MlService";

    public async Task<IReadOnlyDictionary<DateTime, double>> PredictAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default)
    {
        var opts = optionsMonitor.CurrentValue;
        if (string.IsNullOrWhiteSpace(opts.BaseUrl))
            throw new InvalidOperationException("MlService:BaseUrl не задан в конфигурации.");

        var fromUtc = EnsureUtc(from);
        var toUtc = EnsureUtc(to);

        var tasks = await taskRepository.GetByUserAsync(userId, null, null, ct);
        var context = await BuildContextAsync(tasks, userId, DateTime.UtcNow, ct);

        var client = httpClientFactory.CreateClient(ClientName);
        var body = new ProductivityPredictRequestDto
        {
            UserId = userId.ToString(),
            From = fromUtc,
            To = toUtc,
            Context = context
        };

        using var response = await client.PostAsJsonAsync(
            "api/v1/predict/productivity",
            body,
            MlJsonOptions.Instance,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning(
                "ML productivity: {Status} {Body}",
                (int)response.StatusCode,
                detail.Length > 500 ? detail[..500] : detail);
            response.EnsureSuccessStatusCode();
        }

        var payload = await response.Content.ReadFromJsonAsync<ProductivityPredictResponseDto>(MlJsonOptions.Instance, ct);
        var scores = payload?.Scores;
        if (scores == null || scores.Count == 0)
        {
            logger.LogWarning("ML productivity: пустой scores, используем равномерный fallback.");
            return BuildUniformFallback(fromUtc, toUtc);
        }

        var dict = new Dictionary<DateTime, double>();
        foreach (var item in scores)
        {
            var key = NormalizeSlotUtc(item.SlotStart);
            dict[key] = Math.Clamp(item.Score, 0, 1);
        }

        return dict;
    }

    private static DateTime EnsureUtc(DateTime dt) =>
        dt.Kind switch
        {
            DateTimeKind.Utc => dt,
            DateTimeKind.Local => dt.ToUniversalTime(),
            _ => DateTime.SpecifyKind(dt, DateTimeKind.Utc)
        };

    private static DateTime NormalizeSlotUtc(DateTime slotStart)
    {
        if (slotStart.Kind == DateTimeKind.Utc)
            return slotStart;
        if (slotStart.Kind == DateTimeKind.Local)
            return slotStart.ToUniversalTime();
        return DateTime.SpecifyKind(slotStart, DateTimeKind.Utc);
    }

    private async Task<ProductivityContextDto?> BuildContextAsync(
        IReadOnlyList<TaskItem> tasks,
        Guid userId,
        DateTime utcNow,
        CancellationToken ct)
    {
        if (tasks.Count == 0)
            return null;

        var doneWithCompletion = tasks
            .Where(t => t.Status == TaskItemStatus.Done && t.CompletedAt.HasValue)
            .ToList();

        var completedLast7d = doneWithCompletion.Count(t => t.CompletedAt >= utcNow.AddDays(-7));
        var completedLast30d = doneWithCompletion.Count(t => t.CompletedAt >= utcNow.AddDays(-30));

        var cancelledLast30d = tasks.Count(t =>
            t.Status == TaskItemStatus.Cancelled && t.CreatedAt >= utcNow.AddDays(-30));

        var withEst = tasks.Where(t => t.EstimatedMinutes.HasValue).ToList();
        double? avgEstimated = withEst.Count > 0
            ? withEst.Average(t => (double)t.EstimatedMinutes!.Value)
            : null;

        var doneWithActual = doneWithCompletion.Where(t => t.ActualMinutes.HasValue).ToList();
        double? avgActual = doneWithActual.Count > 0
            ? doneWithActual.Average(t => (double)t.ActualMinutes!.Value)
            : null;

        double? actualRatio = doneWithCompletion.Count > 0
            ? (double)doneWithCompletion.Count(t => t.ActualMinutes.HasValue) / doneWithCompletion.Count
            : null;

        var wellbeing30Start = utcNow.AddDays(-30);
        var schedules = await questionnaireRepository.GetUserSchedulesAsync(userId, ct);
        var primaryQuestionnaireId = schedules.FirstOrDefault()?.QuestionnaireId;
        var wellbeingResponses = primaryQuestionnaireId.HasValue
            ? await questionnaireRepository.GetResponsesAsync(userId, primaryQuestionnaireId.Value, 200, ct)
            : [];

        var recent7 = wellbeingResponses.Where(x => x.SubmittedAtUtc >= utcNow.AddDays(-7)).ToList();
        var recent30 = wellbeingResponses.Where(x => x.SubmittedAtUtc >= wellbeing30Start).ToList();
        double? wellbeingAvg7d = recent7.Count > 0 ? recent7.Average(x => x.TotalScore) : null;
        double? wellbeingAvg30d = recent30.Count > 0 ? recent30.Average(x => x.TotalScore) : null;
        double? wellbeingTrend7d = null;
        if (recent7.Count >= 2)
        {
            var ordered = recent7.OrderBy(x => x.SubmittedAtUtc).ToList();
            wellbeingTrend7d = ordered[^1].TotalScore - ordered[0].TotalScore;
        }

        return new ProductivityContextDto
        {
            CompletedLast7d = completedLast7d,
            CompletedLast30d = completedLast30d,
            CancelledLast30d = cancelledLast30d,
            AvgEstimatedMinutes = avgEstimated,
            AvgActualMinutes = avgActual,
            ActualMinutesKnownRatio = actualRatio,
            WellbeingAvg7d = wellbeingAvg7d,
            WellbeingAvg30d = wellbeingAvg30d,
            WellbeingTrend7d = wellbeingTrend7d
        };
    }

    private static IReadOnlyDictionary<DateTime, double> BuildUniformFallback(DateTime fromUtc, DateTime toUtc)
    {
        var result = new Dictionary<DateTime, double>();
        var current = new DateTime(fromUtc.Year, fromUtc.Month, fromUtc.Day, fromUtc.Hour, 0, 0, DateTimeKind.Utc);
        var end = toUtc;
        while (current < end)
        {
            result[current] = 0.5;
            current = current.AddHours(1);
        }

        return result;
    }
}
