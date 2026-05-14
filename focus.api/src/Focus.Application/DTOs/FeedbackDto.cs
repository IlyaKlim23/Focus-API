namespace Focus.Application.DTOs;

public record FeedbackDto(Guid Id, Guid UserId, string Message, int Rating, DateTime CreatedAt);
public record CreateFeedbackRequest(string Message, int Rating);
