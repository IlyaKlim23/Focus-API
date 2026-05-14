namespace Focus.Infrastructure.ML;

internal sealed class ProductivityPredictRequestDto
{
    public string UserId { get; set; } = "";
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public ProductivityContextDto? Context { get; set; }
}

internal sealed class ProductivityContextDto
{
    public int? CompletedLast7d { get; set; }
    public int? CompletedLast30d { get; set; }
    public int? CancelledLast30d { get; set; }
    public double? AvgEstimatedMinutes { get; set; }
    public double? AvgActualMinutes { get; set; }
    public double? ActualMinutesKnownRatio { get; set; }
    public double? WellbeingAvg7d { get; set; }
    public double? WellbeingAvg30d { get; set; }
    public double? WellbeingTrend7d { get; set; }
}

internal sealed class ProductivityPredictResponseDto
{
    public List<ProductivityScoreItemDto>? Scores { get; set; }
}

internal sealed class ProductivityScoreItemDto
{
    public DateTime SlotStart { get; set; }
    public double Score { get; set; }
}

internal sealed class NoteAnalyzeRequestDto
{
    public string Text { get; set; } = "";

    /// <summary>Настроение 1–5; опционально.</summary>
    public int? Mood { get; set; }

    /// <summary>Энергия 1–5; опционально.</summary>
    public int? Energy { get; set; }
}

internal sealed class NoteAnalyzeResponseDto
{
    public List<string>? ExtractedFactors { get; set; }
    public double? SentimentScore { get; set; }
}
