using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

/// <summary>
/// Сервис управления задачами пользователя
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Получает задачу по идентификатору
    /// </summary>
    Task<TaskDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Возвращает список задач пользователя за период
    /// </summary>
    Task<IReadOnlyList<TaskDto>> GetByUserAsync(Guid userId, DateTime? from, DateTime? to, CancellationToken ct = default);

    /// <summary>
    /// Создаёт новую задачу
    /// </summary>
    Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct = default);

    /// <summary>
    /// Обновляет существующую задачу
    /// </summary>
    Task<TaskDto?> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request, CancellationToken ct = default);

    /// <summary>
    /// Удаляет задачу
    /// </summary>
    Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default);
}
