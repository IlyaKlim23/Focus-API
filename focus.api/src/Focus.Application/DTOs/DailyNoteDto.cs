namespace Focus.Application.DTOs;

/// <summary>
/// DTO ежедневной заметки
/// </summary>
/// <param name="Id">Идентификатор</param>
/// <param name="Date">Дата</param>
/// <param name="Content">Текст заметки</param>
/// <param name="MoodScore">Оценка настроения</param>
/// <param name="EnergyLevel">Уровень энергии</param>
/// <param name="ExtractedFactors">Извлечённые NLP-факторы</param>
/// <param name="CreatedAt">Время создания</param>
public record DailyNoteDto(
    Guid Id,
    DateOnly Date,
    string Content,
    int? MoodScore,
    int? EnergyLevel,
    string? ExtractedFactors,
    DateTime CreatedAt);

/// <summary>
/// Запрос на создание или обновление заметки
/// </summary>
/// <param name="Content">Текст заметки</param>
/// <param name="MoodScore">Оценка настроения</param>
/// <param name="EnergyLevel">Уровень энергии</param>
public record CreateDailyNoteRequest(string Content, int? MoodScore, int? EnergyLevel);
