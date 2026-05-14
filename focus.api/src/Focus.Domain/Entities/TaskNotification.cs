namespace Focus.Domain.Entities;

/// <summary>
/// Запись об отправке уведомления по задаче
/// </summary>
public class TaskNotification
{
    /// <summary>
    /// Уникальный идентификатор уведомления
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Идентификатор задачи
    /// </summary>
    public Guid TaskId { get; set; }

    /// <summary>
    /// Начало связанного слота задачи
    /// </summary>
    public DateTime SlotStart { get; set; }

    /// <summary>
    /// Время плановой отправки в UTC
    /// </summary>
    public DateTime ScheduledForUtc { get; set; }

    /// <summary>
    /// Время фактической отправки в UTC
    /// </summary>
    public DateTime? SentAtUtc { get; set; }

    /// <summary>
    /// Статус отправки
    /// </summary>
    public string Status { get; set; } = "Pending";

    /// <summary>
    /// Количество попыток отправки
    /// </summary>
    public int AttemptCount { get; set; }

    /// <summary>
    /// Последний текст ошибки отправки
    /// </summary>
    public string? LastError { get; set; }

    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
