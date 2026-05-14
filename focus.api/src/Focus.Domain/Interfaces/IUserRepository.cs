using Focus.Domain.Entities;

namespace Focus.Domain.Interfaces;

/// <summary>
/// Хранилище пользователей
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Получает пользователя по идентификатору
    /// </summary>
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>
    /// Получает пользователя по email
    /// </summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);

    /// <summary>
    /// Добавляет нового пользователя
    /// </summary>
    Task<User> AddAsync(User user, CancellationToken ct = default);

    /// <summary>
    /// Обновляет пользователя
    /// </summary>
    Task UpdateAsync(User user, CancellationToken ct = default);
}
