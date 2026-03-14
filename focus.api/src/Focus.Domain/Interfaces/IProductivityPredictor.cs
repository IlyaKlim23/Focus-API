namespace Focus.Domain.Interfaces;

/// <summary>
/// Предсказание продуктивности по часовым слотам на основе ML
/// </summary>
public interface IProductivityPredictor
{
    /// <summary>
    /// Возвращает вероятность высокой продуктивности для каждого часового слота в диапазоне
    /// </summary>
    Task<IReadOnlyDictionary<DateTime, double>> PredictAsync(
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken ct = default);
}
