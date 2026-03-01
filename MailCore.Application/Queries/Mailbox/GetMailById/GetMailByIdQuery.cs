using MailCore.Application.DTOs.Mailbox;
using MediatR;

namespace MailCore.Application.Queries.Mailbox.GetMailById
{
    public sealed record GetMailByIdQuery(Guid UserId, Guid MailId) : IRequest<MailboxDetailDto?>;
}