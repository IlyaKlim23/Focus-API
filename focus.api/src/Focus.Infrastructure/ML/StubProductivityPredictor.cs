using Focus.Domain.Interfaces;

namespace Focus.Infrastructure.ML;

/// <summary>
/// Заглушка: возвращает равномерное предсказание. Замените на ML-модель (ML.NET или вызов Python).
/// </summary>
public class StubProductivityPredictor : IProductivityPredictor
{
    public Task<IReadOnlyDictionary<DateTime, double>> PredictAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct = default)
    {
        var result = new Dictionary<DateTime, double>();
        var current = new DateTime(from.Year, from.Month, from.Day, from.Hour, 0, 0, from.Kind);
        var end = to;

        while (current <= end)
        {
            // Простая эвристика: утро и ранний вечер продуктивнее
            var hour = current.Hour;
            var score = hour is >= 9 and <= 11 or >= 14 and <= 17 ? 0.8 : 0.5;
            result[current] = score;
            current = current.AddHours(1);
        }

        return Task.FromResult<IReadOnlyDictionary<DateTime, double>>(result);
    }
}
