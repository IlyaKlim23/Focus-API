namespace Focus.Domain.Entities;

/// <summary>
/// Ответ на отдельный вопрос в сессии опросника
/// </summary>
public class QuestionnaireResponseItem
{
    /// <summary>
    /// Идентификатор ответа
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор сессии ответов
    /// </summary>
    public Guid ResponseId { get; set; }

    /// <summary>
    /// Идентификатор вопроса
    /// </summary>
    public Guid QuestionId { get; set; }

    /// <summary>
    /// Числовое значение ответа
    /// </summary>
    public int Value { get; set; }
}
