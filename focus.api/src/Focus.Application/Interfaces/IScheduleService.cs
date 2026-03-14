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
}
