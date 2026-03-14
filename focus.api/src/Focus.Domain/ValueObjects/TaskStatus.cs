namespace Focus.Domain.ValueObjects;

/// <summary>
/// Статус задачи
/// </summary>
public enum TaskItemStatus
{
    /// <summary>
    /// К выполнению
    /// </summary>
    Todo = 0,

    /// <summary>
    /// В процессе
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Выполнена
    /// </summary>
    Done = 2,

    /// <summary>
    /// Отменена
    /// </summary>
    Cancelled = 3
}
