namespace Focus.Domain.Entities;

/// <summary>
/// Запланированный интервал выполнения задачи (результат планировщика или ручное размещение)
/// </summary>
public class ScheduledSlot
{
    /// <summary>
    /// Идентификатор слота
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
    /// Начало слота (UTC)
    /// </summary>
    public DateTime SlotStart { get; set; }

    /// <summary>
    /// Длительность слота в минутах
    /// </summary>
    public int DurationMinutes { get; set; }

    /// <summary>
    /// Дата и время создания слота
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Навигация на пользователя
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Навигация на задачу
    /// </summary>
    public TaskItem Task { get; set; } = null!;
}
