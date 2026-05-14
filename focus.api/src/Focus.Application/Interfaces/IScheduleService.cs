using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

/// <summary>
/// Сервис генерации оптимального расписания на день
/// </summary>
public interface IScheduleService
{
    /// <summary>
    /// Формирует расписание задач на указанную дату с учётом предсказаний продуктивности
    /// </summary>
    Task<ScheduleResponse> GenerateScheduleAsync(ScheduleRequest request, CancellationToken ct = default);

    Task<ScheduleResponse> GetScheduleAsync(Guid userId, DateTime date, CancellationToken ct = default);
    Task<ScheduleSlotDto> AddManualSlotAsync(Guid userId, CreateScheduleSlotRequest request, CancellationToken ct = default);
    Task<ScheduleSlotDto?> UpdateManualSlotAsync(Guid userId, Guid slotId, UpdateScheduleSlotRequest request, CancellationToken ct = default);
    Task DeleteManualSlotAsync(Guid userId, Guid slotId, CancellationToken ct = default);
}
