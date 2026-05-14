namespace Focus.Application.DTOs;

public record NotificationPreferenceDto(
    string Email,
    bool IsEnabled,
    int RemindBeforeMinutes,
    int? UnavailableFromMinutes,
    int? UnavailableToMinutes);

public record UpsertNotificationPreferenceRequest(
    string Email,
    bool IsEnabled,
    int RemindBeforeMinutes,
    int? UnavailableFromMinutes,
    int? UnavailableToMinutes);
