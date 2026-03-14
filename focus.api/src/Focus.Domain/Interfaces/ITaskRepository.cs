using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

/// <summary>
/// Хранилище задач пользователя
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Получает задачу по идентификатору
    /// </summary>
    Task<TaskItem?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Возвращает список задач пользователя за период
    /// </summary>
    Task<IReadOnlyList<TaskItem>> GetByUserAsync(Guid userId, DateTime? from = null, DateTime? to = null, CancellationToken ct = default);

    /// <summary>
    /// Добавляет новую задачу
    /// </summary>
    Task<TaskItem> AddAsync(TaskItem task, CancellationToken ct = default);

    /// <summary>
    /// Обновляет существующую задачу
    /// </summary>
    Task UpdateAsync(TaskItem task, CancellationToken ct = default);

    /// <summary>
    /// Удаляет задачу
    /// </summary>
    Task DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}
