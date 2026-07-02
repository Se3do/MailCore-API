using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetStarredPaged
{
    public sealed class GetStarredPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetStarredPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetStarredPagedQuery query, CancellationToken ct)
            => repo.GetStarredPagedAsync(query.UserId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
