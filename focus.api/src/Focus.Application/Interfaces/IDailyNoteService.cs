using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

/// <summary>
/// Сервис ежедневных заметок пользователя
/// </summary>
public interface IDailyNoteService
{
    /// <summary>
    /// Получает заметку за указанную дату
    /// </summary>
    Task<DailyNoteDto?> GetByDateAsync(Guid userId, DateOnly date, CancellationToken ct = default);

    /// <summary>
    /// Создаёт или обновляет заметку за день
    /// </summary>
    Task<DailyNoteDto> CreateOrUpdateAsync(Guid userId, DateOnly date, CreateDailyNoteRequest request, CancellationToken ct = default);
}
