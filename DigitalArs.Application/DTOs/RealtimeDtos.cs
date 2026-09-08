namespace DigitalArs.Application.DTOs;

public record AccountRealtimeEvent(
    int UserId,
    decimal Balance,
    string Type,
    decimal Amount,
    string Title,
    string Message,
    DateTime OccurredAt,
    int NotificationId);
