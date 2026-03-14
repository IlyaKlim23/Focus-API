using Focus.Domain.ValueObjects;

namespace Focus.Application.DTOs;

/// <summary>
/// DTO задачи
/// </summary>
/// <param name="Id">Идентификатор</param>
/// <param name="Title">Название</param>
/// <param name="Description">Описание</param>
/// <param name="Status">Статус</param>
/// <param name="Priority">Приоритет</param>
/// <param name="EstimatedMinutes">Оценка длительности в минутах</param>
/// <param name="ActualMinutes">Фактическая длительность в минутах</param>
/// <param name="InterruptionCount">Количество прерываний</param>
/// <param name="DueDate">Крайний срок</param>
/// <param name="StartedAt">Время начала</param>
/// <param name="CompletedAt">Время завершения</param>
/// <param name="CreatedAt">Время создания</param>
/// <param name="CategoryId">Идентификатор категории</param>
public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    TaskPriority Priority,
    int? EstimatedMinutes,
    int? ActualMinutes,
    int InterruptionCount,
    DateTime? DueDate,
    DateTime? StartedAt,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    Guid? CategoryId);

/// <summary>
/// Запрос на создание задачи
/// </summary>
/// <param name="Title">Название</param>
/// <param name="Description">Описание</param>
/// <param name="Priority">Приоритет</param>
/// <param name="EstimatedMinutes">Оценка длительности в минутах</param>
/// <param name="DueDate">Крайний срок</param>
/// <param name="CategoryId">Идентификатор категории</param>
public record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    int? EstimatedMinutes,
    DateTime? DueDate,
    Guid? CategoryId);

/// <summary>
/// Запрос на обновление задачи
/// </summary>
/// <param name="Title">Название</param>
/// <param name="Description">Описание</param>
/// <param name="Status">Статус</param>
/// <param name="Priority">Приоритет</param>
/// <param name="EstimatedMinutes">Оценка длительности в минутах</param>
/// <param name="ActualMinutes">Фактическая длительность в минутах</param>
/// <param name="InterruptionCount">Количество прерываний</param>
/// <param name="DueDate">Крайний срок</param>
/// <param name="CategoryId">Идентификатор категории</param>
public record UpdateTaskRequest(
    string? Title,
    string? Description,
    TaskItemStatus? Status,
    TaskPriority? Priority,
    int? EstimatedMinutes,
    int? ActualMinutes,
    int? InterruptionCount,
    DateTime? DueDate,
    Guid? CategoryId);
