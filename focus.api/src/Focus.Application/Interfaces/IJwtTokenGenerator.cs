using Focus.Domain.Entities;

namespace Focus.Application.Interfaces;

/// <summary>
/// Генерация JWT-токенов для аутентификации
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// Создаёт access-токен для пользователя
    /// </summary>
    /// <param name="user">Пользователь</param>
    /// <returns>Токен и время истечения</returns>
    (string Token, DateTime ExpiresAt) Generate(User user);
}
