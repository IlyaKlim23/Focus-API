namespace Focus.Domain.Entities;

/// <summary>
/// Предсказание продуктивности для часового слота (может кэшироваться)
/// </summary>
public class ProductivityPrediction
{
    /// <summary>
    /// Уникальный идентификатор
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Начало часового слота
    /// </summary>
    public DateTime SlotStart { get; set; }

    /// <summary>
    /// Вероятность высокой продуктивности от 0 до 1
    /// </summary>
    public double Score { get; set; }

    /// <summary>
    /// Объясняющие факторы (JSON)
    /// </summary>
    public string? Factors { get; set; }
}
