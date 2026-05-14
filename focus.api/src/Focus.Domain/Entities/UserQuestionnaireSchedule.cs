namespace Focus.Domain.Entities;

/// <summary>
/// Расписание прохождения опросника для пользователя
/// </summary>
public class UserQuestionnaireSchedule
{
    /// <summary>
    /// Идентификатор расписания
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
    /// Периодичность прохождения
    /// </summary>
    public string Cadence { get; set; } = "Weekly";

    /// <summary>
    /// Следующая дата прохождения в UTC
    /// </summary>
    public DateTime NextDueAtUtc { get; set; }

    /// <summary>
    /// Флаг включения расписания
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
