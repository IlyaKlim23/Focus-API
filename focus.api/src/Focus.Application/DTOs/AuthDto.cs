namespace Focus.Application.DTOs;

/// <summary>
/// Запрос на регистрацию пользователя
/// </summary>
/// <param name="Email">Email для входа</param>
/// <param name="Password">Пароль</param>
/// <param name="DisplayName">Отображаемое имя</param>
public record RegisterRequest(string Email, string Password, string? DisplayName);

/// <summary>
/// Запрос на вход
/// </summary>
/// <param name="Email">Email</param>
/// <param name="Password">Пароль</param>
public record LoginRequest(string Email, string Password);

/// <summary>
/// Запрос на сброс пароля
/// </summary>
/// <param name="Email">Email пользователя</param>
/// <param name="NewPassword">Новый пароль</param>
public record ResetPasswordRequest(string Email, string NewPassword);

/// <summary>
/// Ответ с данными авторизации
/// </summary>
/// <param name="AccessToken">JWT access-токен</param>
/// <param name="UserId">Идентификатор пользователя</param>
/// <param name="Email">Email</param>
/// <param name="DisplayName">Отображаемое имя</param>
/// <param name="Role">Роль пользователя</param>
/// <param name="ExpiresAt">Время истечения токена</param>
public record AuthResponse(string AccessToken, Guid UserId, string Email, string? DisplayName, string Role, DateTime ExpiresAt);
