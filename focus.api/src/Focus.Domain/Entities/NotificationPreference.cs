namespace Focus.Domain.Entities;

/// <summary>
/// Настройки email-оповещений пользователя
/// </summary>
public class NotificationPreference
{
    /// <summary>
    /// Уникальный идентификатор настройки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Email для отправки оповещений
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Флаг включения оповещений
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Количество минут до задачи для напоминания
    /// </summary>
    public int RemindBeforeMinutes { get; set; } = 60;

    /// <summary>
    /// Начало недоступного времени в минутах от начала суток
    /// </summary>
    public int? UnavailableFromMinutes { get; set; }

    /// <summary>
    /// Конец недоступного времени в минутах от начала суток
    /// </summary>
    public int? UnavailableToMinutes { get; set; }

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
