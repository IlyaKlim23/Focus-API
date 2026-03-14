namespace Focus.Domain.Interfaces;

/// <summary>
/// Результат NLP-анализа текста заметки
/// </summary>
/// <param name="ExtractedFactors">Извлечённые ключевые факторы</param>
/// <param name="SentimentScore">Оценка тональности</param>
public record NlpAnalysisResult(IReadOnlyList<string> ExtractedFactors, double? SentimentScore);

/// <summary>
/// Анализ текста заметок для извлечения факторов продуктивности
/// </summary>
public interface INlpAnalyzer
{
    /// <summary>
    /// Анализирует текст и извлекает ключевые факторы (усталость, отвлечения и т.д.)
    /// </summary>
    Task<NlpAnalysisResult> AnalyzeAsync(string text, CancellationToken ct = default);
}
