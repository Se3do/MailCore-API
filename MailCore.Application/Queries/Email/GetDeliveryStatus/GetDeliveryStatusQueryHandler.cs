using MailCore.Application.DTOs.Emails;
using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Email.GetDeliveryStatus;

public class GetDeliveryStatusQueryHandler : IRequestHandler<GetDeliveryStatusQuery, DeliveryStatusDto?>
{
    private readonly IEmailRepository _emailRepository;

    public GetDeliveryStatusQueryHandler(IEmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
    }

    public async Task<DeliveryStatusDto?> Handle(GetDeliveryStatusQuery query, CancellationToken ct)
    {
        var email = await _emailRepository.GetByIdAsync(query.EmailId, ct)
            ?? throw new NotFoundException($"Email {query.EmailId} not found.");

        if (email.SenderId != query.UserId)
            throw new ForbiddenException("You do not have access to this email.");

        return new DeliveryStatusDto(
            email.Id,
            email.DeliveryStatus.ToString(),
            email.SentAt,
            email.SendAttempts,
            email.LastSendError
        );
    }
}
