using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

/// <summary>
/// Хранилище ежедневных заметок
/// </summary>
public interface IDailyNoteRepository
{
    /// <summary>
    /// Получает заметку пользователя за указанную дату
    /// </summary>
    Task<DailyNote?> GetByUserAndDateAsync(Guid userId, DateOnly date, CancellationToken ct = default);

    /// <summary>
    /// Возвращает заметки пользователя за период
    /// </summary>
    Task<IReadOnlyList<DailyNote>> GetByUserAsync(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);

    /// <summary>
    /// Добавляет новую заметку
    /// </summary>
    Task<DailyNote> AddAsync(DailyNote note, CancellationToken ct = default);

    /// <summary>
    /// Обновляет заметку
    /// </summary>
    Task UpdateAsync(DailyNote note, CancellationToken ct = default);
}
