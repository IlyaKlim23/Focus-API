namespace Focus.Domain.Entities;

/// <summary>
/// Обратная связь пользователя о работе приложения
/// </summary>
public class UserFeedback
{
    /// <summary>
    /// Идентификатор отзыва
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Текст обратной связи
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Оценка от 1 до 5
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
