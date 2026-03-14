namespace Focus.Application.DTOs;

/// <summary>
/// Предсказание продуктивности для слота
/// </summary>
/// <param name="SlotStart">Начало часового слота</param>
/// <param name="Score">Вероятность высокой продуктивности</param>
/// <param name="Factors">Объясняющие факторы</param>
public record ProductivityPredictionDto(DateTime SlotStart, double Score, string? Factors);

/// <summary>
/// Ответ с предсказаниями продуктивности
/// </summary>
/// <param name="Date">Дата</param>
/// <param name="Predictions">Предсказания по слотам</param>
public record ProductivityResponse(DateTime Date, IReadOnlyList<ProductivityPredictionDto> Predictions);
