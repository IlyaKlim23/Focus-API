using Focus.Application.DTOs;

namespace Focus.Application.Interfaces;

/// <summary>
/// Сервис аутентификации и регистрации пользователей
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрирует нового пользователя
    /// </summary>
    /// <param name="request">Данные для регистрации</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат авторизации с токеном или null, если email уже занят</returns>
    Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default);

    /// <summary>
    /// Выполняет вход по email и паролю
    /// </summary>
    /// <param name="request">Учётные данные</param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Результат авторизации с токеном или null при неверных данных</returns>
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);

    /// <summary>
    /// Сбрасывает пароль пользователя по email
    /// </summary>
    Task<bool> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct = default);
}
