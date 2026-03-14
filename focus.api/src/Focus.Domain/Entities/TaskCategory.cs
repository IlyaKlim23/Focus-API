namespace Focus.Domain.Entities;

/// <summary>
/// Категория задач пользователя (для группировки и признаков ML)
/// </summary>
public class TaskCategory
{
    /// <summary>
    /// Уникальный идентификатор категории
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор владельца
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Цвет для отображения (hex или название)
    /// </summary>
    public string? Color { get; set; }
}
