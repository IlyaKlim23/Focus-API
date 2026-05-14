using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using Focus.Application.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Focus.Infrastructure.Telemetry;

public interface IClickHouseTelemetrySink
{
    void EnqueueLog(string level, string category, string message, string? exception);
    void EnqueueMetric(string name, double value, string? route, int statusCode, double durationMs);
    void EnqueueQuestionnaireIndicator(Guid userId, Guid questionnaireId, int totalScore, int answersCount);
}

public class ClickHouseTelemetrySink(
    IHttpClientFactory httpClientFactory,
    IOptions<ClickHouseOptions> options) : BackgroundService, IClickHouseTelemetrySink, ITelemetryWriter
{
    private readonly Channel<(string Kind, object Payload)> _queue = Channel.CreateUnbounded<(string Kind, object Payload)>();
    private readonly ClickHouseOptions _cfg = options.Value;

    public void EnqueueLog(string level, string category, string message, string? exception)
    {
        if (!_cfg.Enabled) return;
        _queue.Writer.TryWrite(("log", new
        {
            ts = DateTime.UtcNow,
            level,
            category,
            message,
            exception = exception ?? string.Empty
        }));
    }

    public void EnqueueMetric(string name, double value, string? route, int statusCode, double durationMs)
    {
        if (!_cfg.Enabled) return;
        _queue.Writer.TryWrite(("metric", new
        {
            ts = DateTime.UtcNow,
            name,
            value,
            route = route ?? string.Empty,
            statusCode,
            durationMs
        }));
    }

    public void EnqueueQuestionnaireIndicator(Guid userId, Guid questionnaireId, int totalScore, int answersCount)
    {
        if (!_cfg.Enabled) return;
        _queue.Writer.TryWrite(("questionnaire", new
        {
            ts = DateTime.UtcNow,
            userId = userId.ToString(),
            questionnaireId = questionnaireId.ToString(),
            totalScore,
            answersCount
        }));
    }

    public void TrackQuestionnaireIndicator(Guid userId, Guid questionnaireId, int totalScore, int answersCount) =>
        EnqueueQuestionnaireIndicator(userId, questionnaireId, totalScore, answersCount);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_cfg.Enabled) return;
        await EnsureSchemaAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            var item = await _queue.Reader.ReadAsync(stoppingToken);
            try
            {
                await WriteAsync(item.Kind, item.Payload, stoppingToken);
            }
            catch
            {
                // Не ломаем основной pipeline приложения из-за телеметрии.
            }
        }
    }

    private async Task EnsureSchemaAsync(CancellationToken ct)
    {
        var createLogs = """
            CREATE TABLE IF NOT EXISTS app_logs (
                ts DateTime,
                level String,
                category String,
                message String,
                exception String
            ) ENGINE = MergeTree
            ORDER BY (ts, level, category);
            """;

        var createMetrics = """
            CREATE TABLE IF NOT EXISTS app_metrics (
                ts DateTime,
                name String,
                value Float64,
                route String,
                statusCode UInt16,
                durationMs Float64
            ) ENGINE = MergeTree
            ORDER BY (ts, name, route);
            """;
        var createQuestionnaireIndicators = """
            CREATE TABLE IF NOT EXISTS questionnaire_indicators (
                ts DateTime,
                userId String,
                questionnaireId String,
                totalScore Int32,
                answersCount Int32
            ) ENGINE = MergeTree
            ORDER BY (ts, questionnaireId, userId);
            """;

        await ExecuteCommandAsync(createLogs, ct);
        await ExecuteCommandAsync(createMetrics, ct);
        await ExecuteCommandAsync(createQuestionnaireIndicators, ct);
    }

    private async Task WriteAsync(string kind, object payload, CancellationToken ct)
    {
        var table = kind switch
        {
            "log" => "app_logs",
            "metric" => "app_metrics",
            "questionnaire" => "questionnaire_indicators",
            _ => "app_metrics"
        };
        var jsonLine = JsonSerializer.Serialize(payload);
        var sql = $"INSERT INTO {_cfg.Database}.{table} FORMAT JSONEachRow\n{jsonLine}";
        await ExecuteCommandAsync(sql, ct);
    }

    private async Task ExecuteCommandAsync(string sql, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient("ClickHouse");
        var request = new HttpRequestMessage(HttpMethod.Post, "/")
        {
            Content = new StringContent(sql, Encoding.UTF8, "text/plain")
        };
        await client.SendAsync(request, ct);
    }
}
