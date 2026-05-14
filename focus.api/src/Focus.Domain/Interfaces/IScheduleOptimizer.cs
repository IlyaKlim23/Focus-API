namespace Focus.Domain.Interfaces;

/// <summary>
/// Задача, размещённая в слоте расписания
/// </summary>
/// <param name="TaskId">Идентификатор задачи</param>
/// <param name="SlotStart">Время начала слота</param>
/// <param name="DurationMinutes">Длительность в минутах</param>
public record ScheduledTask(Guid TaskId, DateTime SlotStart, int DurationMinutes);

/// <summary>
/// Алгоритм формирования оптимального расписания
/// </summary>
public interface IScheduleOptimizer
{
    /// <summary>
    /// Распределяет задачи по слотам с учётом предсказаний продуктивности и приоритетов
    /// </summary>
    IReadOnlyList<ScheduledTask> Optimize(
        IReadOnlyList<TaskInput> tasks,
        IReadOnlyDictionary<DateTime, double> productivityScores,
        DateTime dayStart,
        DateTime dayEnd,
        IReadOnlyList<DailyUnavailableWindow>? unavailableWindows = null);
}

/// <summary>
/// Входные данные задачи для алгоритма планирования
/// </summary>
/// <param name="Id">Идентификатор задачи</param>
/// <param name="Priority">Приоритет</param>
/// <param name="EstimatedMinutes">Оценка длительности в минутах</param>
/// <param name="DueDate">Крайний срок</param>
public record TaskInput(Guid Id, int Priority, int EstimatedMinutes, DateTime? DueDate);

/// <summary>
/// Ежедневное окно недоступности для планирования
/// </summary>
/// <param name="FromMinute">Начало в минутах от 00:00</param>
/// <param name="ToMinute">Конец в минутах от 00:00</param>
public record DailyUnavailableWindow(int FromMinute, int ToMinute);
