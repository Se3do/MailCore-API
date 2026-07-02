using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetSpamPaged
{
    public sealed class GetSpamPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetSpamPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetSpamPagedQuery query, CancellationToken ct)
            => repo.GetSpamPagedAsync(query.UserId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
