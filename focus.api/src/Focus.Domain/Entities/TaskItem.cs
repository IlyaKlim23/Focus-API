using Focus.Domain.ValueObjects;

namespace Focus.Domain.Entities;

/// <summary>
/// Задача пользователя. Хранит метаданные для ML: время выполнения, прерывания, статус
/// </summary>
public class TaskItem
{
    /// <summary>
    /// Уникальный идентификатор задачи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор владельца
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Идентификатор категории (если задана)
    /// </summary>
    public Guid? CategoryId { get; set; }

    /// <summary>
    /// Название задачи
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Подробное описание
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Текущий статус задачи
    /// </summary>
    public TaskItemStatus Status { get; set; }

    /// <summary>
    /// Приоритет для планирования
    /// </summary>
    public TaskPriority Priority { get; set; }

    /// <summary>
    /// Оценка длительности в минутах
    /// </summary>
    public int? EstimatedMinutes { get; set; }

    /// <summary>
    /// Фактическая длительность в минутах
    /// </summary>
    public int? ActualMinutes { get; set; }

    /// <summary>
    /// Количество прерываний (фича для ML)
    /// </summary>
    public int InterruptionCount { get; set; }

    /// <summary>
    /// Крайний срок выполнения
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Время начала работы над задачей
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Время завершения задачи
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Навигация на владельца
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Навигация на категорию
    /// </summary>
    public TaskCategory? Category { get; set; }
}
