using MailService.Application.DTOs.Emails;
using MediatR;

namespace MailService.Application.Queries.Email.GetSentById
{
    public record GetSentByIdQuery(Guid UserId, Guid EmailId): IRequest<EmailDto?>;
}
