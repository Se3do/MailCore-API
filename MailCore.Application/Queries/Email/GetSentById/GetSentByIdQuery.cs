using MailCore.Application.DTOs.Emails;
using MediatR;

namespace MailCore.Application.Queries.Email.GetSentById
{
    public record GetSentByIdQuery(Guid UserId, Guid EmailId): IRequest<EmailDto?>;
}
