using MailCore.Application.DTOs.Emails;
using MediatR;

namespace MailCore.Application.Notifications;

public record EmailSentNotification(
    string SenderEmail,
    IReadOnlyList<string> ToRecipients,
    EmailSummaryDto Email
) : INotification;
