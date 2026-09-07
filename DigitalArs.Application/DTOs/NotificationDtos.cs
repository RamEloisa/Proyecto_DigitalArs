namespace DigitalArs.Application.DTOs;

public record NotificationDto(
    int Id,
    string Type,
    string Title,
    string Message,
    decimal Amount,
    bool IsRead,
    DateTime CreatedAt);
