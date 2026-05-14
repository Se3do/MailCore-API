using MailCore.API.Hubs;
using MailCore.Application.Notifications;
using MailCore.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace MailCore.API.Notifications;

public class EmailSentNotificationHandler : INotificationHandler<EmailSentNotification>
{
    private readonly IHubContext<MailHub> _hubContext;
    private readonly IUserRepository _userRepository;

    public EmailSentNotificationHandler(IHubContext<MailHub> hubContext, IUserRepository userRepository)
    {
        _hubContext = hubContext;
        _userRepository = userRepository;
    }

    public async Task Handle(EmailSentNotification notification, CancellationToken ct)
    {
        foreach (var email in notification.ToRecipients)
        {
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user is null) continue;

            await _hubContext.Clients
                .Group(user.Id.ToString())
                .SendAsync("NewEmail", notification.Email, ct);
        }
    }
}
