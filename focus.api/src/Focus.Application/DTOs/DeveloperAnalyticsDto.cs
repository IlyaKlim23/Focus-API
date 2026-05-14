namespace Focus.Application.DTOs;

public record DailyQuestionnairePoint(DateOnly Date, double AvgScore, int Submissions);
public record QuestionnaireBreakdownItem(string QuestionnaireId, double AvgScore, int Submissions);

public record DeveloperQuestionnaireAnalyticsResponse(
    IReadOnlyList<DailyQuestionnairePoint> DailyTrend,
    IReadOnlyList<QuestionnaireBreakdownItem> ByQuestionnaire);
