using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class ScheduleService(
    ITaskRepository taskRepository,
    IProductivityPredictor predictor,
    IScheduleOptimizer optimizer) : IScheduleService
{
    public async Task<ScheduleResponse> GenerateScheduleAsync(ScheduleRequest request, CancellationToken ct = default)
    {
        var dayStart = new DateTime(request.Date.Year, request.Date.Month, request.Date.Day, 0, 0, 0, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);

        // Получаем все задачи пользователя (для расписания берём незавершённые)
        var tasks = await taskRepository.GetByUserAsync(request.UserId, null, null, ct);
        var pendingTasks = tasks
            .Where(t => t.Status != Domain.ValueObjects.TaskItemStatus.Done && t.Status != Domain.ValueObjects.TaskItemStatus.Cancelled)
            .Select(t => new TaskInput(t.Id, (int)t.Priority, t.EstimatedMinutes ?? 60, t.DueDate))
            .ToList();

        var predictions = await predictor.PredictAsync(request.UserId, dayStart, dayEnd, ct);
        var scheduled = optimizer.Optimize(pendingTasks, predictions, dayStart, dayEnd);

        var taskDict = tasks.ToDictionary(t => t.Id);
        var slots = scheduled
            .Select(s => new ScheduleSlotDto(
                s.SlotStart,
                s.TaskId,
                taskDict.GetValueOrDefault(s.TaskId)?.Title ?? "Unknown",
                s.DurationMinutes))
            .ToList();

        return new ScheduleResponse(request.Date, slots);
    }
}
