namespace Focus.Domain.Entities;

/// <summary>
/// Сессия ответов пользователя на опросник
/// </summary>
public class QuestionnaireResponse
{
    /// <summary>
    /// Идентификатор сессии ответов
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Идентификатор опросника
    /// </summary>
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// Время отправки ответов в UTC
    /// </summary>
    public DateTime SubmittedAtUtc { get; set; }

    /// <summary>
    /// Суммарный балл по ответам
    /// </summary>
    public int TotalScore { get; set; }

    /// <summary>
    /// Дата и время создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
