using Focus.Application.DTOs;
using Focus.Application.Interfaces;
using Focus.Domain.Interfaces;

namespace Focus.Application.Services;

public class ScheduleService(
    ITaskRepository taskRepository,
    IScheduledSlotRepository scheduledSlotRepository,
    IProductivityPredictor predictor,
    IScheduleOptimizer optimizer,
    INotificationPreferenceRepository notificationPreferenceRepository) : IScheduleService
{
    private const int DefaultUnavailableFromMinutes = 22 * 60;
    private const int DefaultUnavailableToMinutes = 6 * 60;

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
        var unavailableWindows = await GetUnavailableWindowsAsync(request.UserId, ct);
        var scheduled = optimizer.Optimize(pendingTasks, predictions, dayStart, dayEnd, unavailableWindows);

        var taskDict = tasks.ToDictionary(t => t.Id);
        var slots = scheduled
            .Select(s => new ScheduleSlotDto(
                Guid.NewGuid(),
                s.SlotStart,
                s.TaskId,
                taskDict.GetValueOrDefault(s.TaskId)?.Title ?? "Unknown",
                s.DurationMinutes))
            .ToList();

        var persisted = slots.Select(x => new Focus.Domain.Entities.ScheduledSlot
        {
            Id = x.Id,
            UserId = request.UserId,
            TaskId = x.TaskId,
            SlotStart = DateTime.SpecifyKind(x.SlotStart, DateTimeKind.Utc),
            DurationMinutes = x.DurationMinutes,
            CreatedAt = DateTime.UtcNow
        }).ToList();
        await scheduledSlotRepository.ReplaceDayAsync(request.UserId, dayStart, dayEnd, persisted, ct);

        return new ScheduleResponse(request.Date, slots);
    }

    public async Task<ScheduleResponse> GetScheduleAsync(Guid userId, DateTime date, CancellationToken ct = default)
    {
        var dayStart = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        var dayEnd = dayStart.AddDays(1);
        var tasks = await taskRepository.GetByUserAsync(userId, null, null, ct);
        var taskDict = tasks.ToDictionary(x => x.Id, x => x.Title);
        var slots = (await scheduledSlotRepository.GetByUserAndDayAsync(userId, dayStart, dayEnd, ct))
            .Select(x => new ScheduleSlotDto(x.Id, x.SlotStart, x.TaskId, taskDict.GetValueOrDefault(x.TaskId) ?? "Unknown", x.DurationMinutes))
            .ToList();
        return new ScheduleResponse(date, slots);
    }

    public async Task<ScheduleSlotDto> AddManualSlotAsync(Guid userId, CreateScheduleSlotRequest request, CancellationToken ct = default)
    {
        var slot = new Focus.Domain.Entities.ScheduledSlot
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TaskId = request.TaskId,
            SlotStart = DateTime.SpecifyKind(request.SlotStart, DateTimeKind.Utc),
            DurationMinutes = Math.Max(5, request.DurationMinutes),
            CreatedAt = DateTime.UtcNow
        };
        await scheduledSlotRepository.AddAsync(slot, ct);
        var task = await taskRepository.GetByIdAsync(request.TaskId, userId, ct);
        return new ScheduleSlotDto(slot.Id, slot.SlotStart, slot.TaskId, task?.Title ?? "Unknown", slot.DurationMinutes);
    }

    public async Task<ScheduleSlotDto?> UpdateManualSlotAsync(Guid userId, Guid slotId, UpdateScheduleSlotRequest request, CancellationToken ct = default)
    {
        var slot = await scheduledSlotRepository.GetByIdAsync(slotId, userId, ct);
        if (slot == null) return null;
        if (request.SlotStart.HasValue) slot.SlotStart = DateTime.SpecifyKind(request.SlotStart.Value, DateTimeKind.Utc);
        if (request.TaskId.HasValue) slot.TaskId = request.TaskId.Value;
        if (request.DurationMinutes.HasValue) slot.DurationMinutes = Math.Max(5, request.DurationMinutes.Value);
        await scheduledSlotRepository.UpdateAsync(slot, ct);
        var task = await taskRepository.GetByIdAsync(slot.TaskId, userId, ct);
        return new ScheduleSlotDto(slot.Id, slot.SlotStart, slot.TaskId, task?.Title ?? "Unknown", slot.DurationMinutes);
    }

    public Task DeleteManualSlotAsync(Guid userId, Guid slotId, CancellationToken ct = default) =>
        scheduledSlotRepository.DeleteAsync(slotId, userId, ct);

    private async Task<IReadOnlyList<DailyUnavailableWindow>> GetUnavailableWindowsAsync(Guid userId, CancellationToken ct)
    {
        var preference = await notificationPreferenceRepository.GetByUserIdAsync(userId, ct);
        if (preference?.UnavailableFromMinutes is not int from || preference.UnavailableToMinutes is not int to)
            return [new DailyUnavailableWindow(DefaultUnavailableFromMinutes, DefaultUnavailableToMinutes)];
        return [new DailyUnavailableWindow(from, to)];
    }
}
