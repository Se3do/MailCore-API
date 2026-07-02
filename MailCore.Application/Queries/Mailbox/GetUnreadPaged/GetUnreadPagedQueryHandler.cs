using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetUnreadPaged
{
    public sealed class GetUnreadPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetUnreadPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetUnreadPagedQuery query, CancellationToken ct)
            => repo.GetUnreadPagedAsync(query.UserId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
