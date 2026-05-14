namespace Focus.Domain.Entities;

/// <summary>
/// Вопрос психологического опросника
/// </summary>
public class PsychologicalQuestionnaireQuestion
{
    /// <summary>
    /// Идентификатор вопроса
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор опросника
    /// </summary>
    public Guid QuestionnaireId { get; set; }

    /// <summary>
    /// Текст вопроса
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Порядок отображения вопроса
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Минимальное значение ответа
    /// </summary>
    public int MinValue { get; set; } = 1;

    /// <summary>
    /// Максимальное значение ответа
    /// </summary>
    public int MaxValue { get; set; } = 5;
}
