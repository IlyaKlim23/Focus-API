namespace Focus.Application.Interfaces;

/// <summary>
/// Хэширование и проверка паролей
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Вычисляет хэш пароля
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Проверяет соответствие пароля хэшу
    /// </summary>
    bool Verify(string password, string hash);
}
