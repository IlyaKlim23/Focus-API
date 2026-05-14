using System.Net.Http.Json;
using Focus.Domain.Interfaces;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Focus.Infrastructure.ML;

namespace Focus.Infrastructure.Nlp;

public class HttpNlpAnalyzer(
    IHttpClientFactory httpClientFactory,
    IOptionsMonitor<MlServiceOptions> optionsMonitor,
    ILogger<HttpNlpAnalyzer> logger) : INlpAnalyzer
{
    private const string ClientName = "MlService";

    public async Task<NlpAnalysisResult> AnalyzeAsync(
        string text,
        int? mood = null,
        int? energy = null,
        CancellationToken ct = default)
    {
        var opts = optionsMonitor.CurrentValue;
        if (string.IsNullOrWhiteSpace(opts.BaseUrl))
            throw new InvalidOperationException("MlService:BaseUrl не задан в конфигурации.");

        var client = httpClientFactory.CreateClient(ClientName);
        var body = new NoteAnalyzeRequestDto
        {
            Text = text,
            Mood = mood is >= 1 and <= 5 ? mood : null,
            Energy = energy is >= 1 and <= 5 ? energy : null
        };

        using var response = await client.PostAsJsonAsync(
            "api/v1/analyze/note",
            body,
            MlJsonOptions.Instance,
            ct);

        if (!response.IsSuccessStatusCode)
        {
            var detail = await response.Content.ReadAsStringAsync(ct);
            logger.LogWarning(
                "ML NLP: {Status} {Body}",
                (int)response.StatusCode,
                detail.Length > 500 ? detail[..500] : detail);
            response.EnsureSuccessStatusCode();
        }

        var payload = await response.Content.ReadFromJsonAsync<NoteAnalyzeResponseDto>(MlJsonOptions.Instance, ct);
        var factors = (IReadOnlyList<string>)(payload?.ExtractedFactors ?? []);
        return new NlpAnalysisResult(factors, payload?.SentimentScore);
    }
}
