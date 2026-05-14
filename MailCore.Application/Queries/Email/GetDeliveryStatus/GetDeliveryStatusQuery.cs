using MailCore.Application.DTOs.Emails;
using MediatR;

namespace MailCore.Application.Queries.Email.GetDeliveryStatus;

public record GetDeliveryStatusQuery(Guid UserId, Guid EmailId) : IRequest<DeliveryStatusDto?>;
