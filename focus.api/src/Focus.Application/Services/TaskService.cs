using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Entities;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class TaskService(ITaskRepository repository) : ITaskService
{
    public async Task<TaskDto?> GetByIdAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        var task = await repository.GetByIdAsync(id, userId, ct);
        return task == null ? null : MapToDto(task);
    }

    public async Task<IReadOnlyList<TaskDto>> GetByUserAsync(Guid userId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var tasks = await repository.GetByUserAsync(userId, from, to, ct);
        return tasks.Select(MapToDto).ToList();
    }

    public async Task<TaskDto> CreateAsync(Guid userId, CreateTaskRequest request, CancellationToken ct = default)
    {
        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            EstimatedMinutes = request.EstimatedMinutes,
            DueDate = request.DueDate,
            CategoryId = request.CategoryId,
            Status = Domain.ValueObjects.TaskItemStatus.Todo,
            CreatedAt = DateTime.UtcNow
        };
        await repository.AddAsync(task, ct);
        return MapToDto(task);
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, Guid userId, UpdateTaskRequest request, CancellationToken ct = default)
    {
        var task = await repository.GetByIdAsync(id, userId, ct);
        if (task == null) return null;

        if (request.Title != null) task.Title = request.Title;
        if (request.Description != null) task.Description = request.Description;
        if (request.Status.HasValue) task.Status = request.Status.Value;
        if (request.Priority.HasValue) task.Priority = request.Priority.Value;
        if (request.EstimatedMinutes.HasValue) task.EstimatedMinutes = request.EstimatedMinutes.Value;
        if (request.ActualMinutes.HasValue) task.ActualMinutes = request.ActualMinutes.Value;
        if (request.InterruptionCount.HasValue) task.InterruptionCount = request.InterruptionCount.Value;
        if (request.DueDate.HasValue) task.DueDate = request.DueDate;
        if (request.CategoryId.HasValue) task.CategoryId = request.CategoryId;

        if (request.Status == Domain.ValueObjects.TaskItemStatus.Done)
            task.CompletedAt = DateTime.UtcNow;

        await repository.UpdateAsync(task, ct);
        return MapToDto(task);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId, CancellationToken ct = default)
    {
        await repository.DeleteAsync(id, userId, ct);
        return true;
    }

    private static TaskDto MapToDto(TaskItem t) => new(
        t.Id, t.Title, t.Description, t.Status, t.Priority,
        t.EstimatedMinutes, t.ActualMinutes, t.InterruptionCount,
        t.DueDate, t.StartedAt, t.CompletedAt, t.CreatedAt, t.CategoryId);
}
