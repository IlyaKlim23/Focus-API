namespace Focus.Domain.Entities;

/// <summary>
/// Пользователь системы планировщика
/// </summary>
public class User
{
    /// <summary>
    /// Уникальный идентификатор пользователя
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Email для входа (уникальный)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое имя
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Хэш пароля (BCrypt)
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время регистрации
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последней активности
    /// </summary>
    public DateTime? LastActiveAt { get; set; }
}
