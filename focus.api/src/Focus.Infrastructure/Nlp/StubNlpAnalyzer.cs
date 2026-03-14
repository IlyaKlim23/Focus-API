using Focus.Domain.Interfaces;

namespace Focus.Infrastructure.Nlp;

/// <summary>
/// Заглушка: возвращает пустой анализ. Замените на ML.NET Text или вызов Python (spaCy).
/// </summary>
public class StubNlpAnalyzer : INlpAnalyzer
{
    public Task<NlpAnalysisResult> AnalyzeAsync(string text, CancellationToken ct = default)
    {
        var factors = new List<string>();
        if (text.Contains("устал", StringComparison.OrdinalIgnoreCase)) factors.Add("tired");
        if (text.Contains("отвлек", StringComparison.OrdinalIgnoreCase)) factors.Add("distracted");
        if (text.Contains("хорошо", StringComparison.OrdinalIgnoreCase) || text.Contains("продуктив", StringComparison.OrdinalIgnoreCase))
            factors.Add("productive");
        return Task.FromResult(new NlpAnalysisResult(factors, null));
    }
}
