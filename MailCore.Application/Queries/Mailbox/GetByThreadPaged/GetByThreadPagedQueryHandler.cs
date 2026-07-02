using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetByThreadPaged
{
    public sealed class GetByThreadPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetByThreadPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetByThreadPagedQuery query, CancellationToken ct)
            => repo.GetByThreadPagedAsync(query.UserId, query.ThreadId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
