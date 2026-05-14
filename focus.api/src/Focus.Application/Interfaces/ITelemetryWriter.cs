namespace Focus.Application.Interfaces;

public interface ITelemetryWriter
{
    void TrackQuestionnaireIndicator(Guid userId, Guid questionnaireId, int totalScore, int answersCount);
}
