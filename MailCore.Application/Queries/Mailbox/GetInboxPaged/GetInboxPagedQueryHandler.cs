using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetInboxPaged
{
    public sealed class GetInboxPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetInboxPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetInboxPagedQuery query, CancellationToken ct)
            => repo.GetInboxPagedAsync(query.UserId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
