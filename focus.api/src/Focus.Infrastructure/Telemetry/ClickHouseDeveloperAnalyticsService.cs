using System.Text.Json;
using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Focus.Infrastructure.Telemetry;

public class ClickHouseDeveloperAnalyticsService(
    IHttpClientFactory httpClientFactory,
    IOptions<ClickHouseOptions> options) : IDeveloperAnalyticsService
{
    public async Task<DeveloperQuestionnaireAnalyticsResponse> GetQuestionnaireAnalyticsAsync(int days = 30, CancellationToken ct = default)
    {
        var cfg = options.Value;
        if (!cfg.Enabled)
            return new DeveloperQuestionnaireAnalyticsResponse([], []);

        var safeDays = Math.Clamp(days, 1, 365);

        var dailySql = $"""
            SELECT toDate(ts) AS day, avg(totalScore) AS avgScore, count() AS submissions
            FROM {cfg.Database}.questionnaire_indicators
            WHERE ts >= now() - INTERVAL {safeDays} DAY
            GROUP BY day
            ORDER BY day
            FORMAT JSON
            """;

        var byQuestionnaireSql = $"""
            SELECT questionnaireId, avg(totalScore) AS avgScore, count() AS submissions
            FROM {cfg.Database}.questionnaire_indicators
            WHERE ts >= now() - INTERVAL {safeDays} DAY
            GROUP BY questionnaireId
            ORDER BY submissions DESC
            FORMAT JSON
            """;

        var dailyJson = await ExecuteSqlAsync(dailySql, ct);
        var byQuestionnaireJson = await ExecuteSqlAsync(byQuestionnaireSql, ct);

        var daily = ParseDaily(dailyJson);
        var byQuestionnaire = ParseByQuestionnaire(byQuestionnaireJson);
        return new DeveloperQuestionnaireAnalyticsResponse(daily, byQuestionnaire);
    }

    private async Task<string> ExecuteSqlAsync(string sql, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient("ClickHouse");
        using var request = new HttpRequestMessage(HttpMethod.Post, "/")
        {
            Content = new StringContent(sql)
        };
        using var response = await client.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(ct);
    }

    private static IReadOnlyList<DailyQuestionnairePoint> ParseDaily(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            return [];

        var result = new List<DailyQuestionnairePoint>();
        foreach (var item in data.EnumerateArray())
        {
            var dayRaw = item.GetProperty("day").GetString() ?? "";
            if (!DateOnly.TryParse(dayRaw, out var day))
                continue;
            var avg = item.GetProperty("avgScore").GetDouble();
            var submissions = item.GetProperty("submissions").GetInt32();
            result.Add(new DailyQuestionnairePoint(day, avg, submissions));
        }
        return result;
    }

    private static IReadOnlyList<QuestionnaireBreakdownItem> ParseByQuestionnaire(string json)
    {
        using var doc = JsonDocument.Parse(json);
        if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Array)
            return [];

        var result = new List<QuestionnaireBreakdownItem>();
        foreach (var item in data.EnumerateArray())
        {
            var id = item.GetProperty("questionnaireId").GetString() ?? "unknown";
            var avg = item.GetProperty("avgScore").GetDouble();
            var submissions = item.GetProperty("submissions").GetInt32();
            result.Add(new QuestionnaireBreakdownItem(id, avg, submissions));
        }
        return result;
    }
}
