using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetTrashPaged
{
    public sealed class GetTrashPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetTrashPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetTrashPagedQuery query, CancellationToken ct)
            => repo.GetDeletedPagedAsync(query.UserId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
