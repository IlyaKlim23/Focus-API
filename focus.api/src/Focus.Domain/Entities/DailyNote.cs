namespace Focus.Domain.Entities;

/// <summary>
/// Ежедневная заметка пользователя. Используется для NLP-анализа и улучшения прогноза продуктивности
/// </summary>
public class DailyNote
{
    /// <summary>
    /// Уникальный идентификатор заметки
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор владельца
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Дата заметки
    /// </summary>
    public DateOnly Date { get; set; }

    /// <summary>
    /// Текст заметки (успехи, причины отвлечений, самочувствие)
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Оценка настроения от 1 до 5 (опционально)
    /// </summary>
    public int? MoodScore { get; set; }

    /// <summary>
    /// Уровень энергии от 1 до 5 (опционально)
    /// </summary>
    public int? EnergyLevel { get; set; }

    /// <summary>
    /// Ключевые факторы, извлечённые NLP (JSON или разделённые)
    /// </summary>
    public string? ExtractedFactors { get; set; }

    /// <summary>
    /// Дата и время создания
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Навигация на владельца
    /// </summary>
    public User User { get; set; } = null!;
}
