namespace Focus.Domain.Entities;

/// <summary>
/// Справочник психологических опросников
/// </summary>
public class PsychologicalQuestionnaire
{
    /// <summary>
    /// Идентификатор опросника
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Короткий код опросника
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Название опросника
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Описание опросника
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Флаг активности опросника
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
