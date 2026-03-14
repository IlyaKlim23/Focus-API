namespace Focus.Application.DTOs;

/// <summary>
/// Запрос на генерацию расписания
/// </summary>
/// <param name="Date">Дата для расписания</param>
/// <param name="UserId">Идентификатор пользователя</param>
public record ScheduleRequest(DateTime Date, Guid UserId);

/// <summary>
/// Слот в расписании (задача с привязкой ко времени)
/// </summary>
/// <param name="SlotStart">Время начала слота</param>
/// <param name="TaskId">Идентификатор задачи</param>
/// <param name="TaskTitle">Название задачи</param>
/// <param name="DurationMinutes">Длительность в минутах</param>
public record ScheduleSlotDto(DateTime SlotStart, Guid TaskId, string TaskTitle, int DurationMinutes);

/// <summary>
/// Ответ с расписанием на день
/// </summary>
/// <param name="Date">Дата</param>
/// <param name="Slots">Слоты расписания</param>
public record ScheduleResponse(DateTime Date, IReadOnlyList<ScheduleSlotDto> Slots);
