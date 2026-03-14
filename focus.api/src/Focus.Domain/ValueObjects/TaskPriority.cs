namespace Focus.Domain.ValueObjects;

/// <summary>
/// Приоритет задачи (для алгоритма планирования)
/// </summary>
public enum TaskPriority
{
    /// <summary>
    /// Низкий приоритет
    /// </summary>
    Low = 0,

    /// <summary>
    /// Средний приоритет
    /// </summary>
    Medium = 1,

    /// <summary>
    /// Высокий приоритет
    /// </summary>
    High = 2,

    /// <summary>
    /// Критический приоритет
    /// </summary>
    Critical = 3
}
