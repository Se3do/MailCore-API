namespace MailCore.Application.DTOs.Emails;

/// <summary>Delivery status and send history for an email message.</summary>
public sealed record DeliveryStatusDto(
    Guid Id,
    string Status,
    DateTime? SentAt,
    int SendAttempts,
    string? LastSendError
);
