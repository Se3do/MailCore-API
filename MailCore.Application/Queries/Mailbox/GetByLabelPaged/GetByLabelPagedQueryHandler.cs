using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;

namespace MailCore.Application.Queries.Mailbox.GetByLabelPaged
{
    public sealed class GetByLabelPagedQueryHandler(IMailRecipientRepository repo)
        : BaseMailboxPagedQueryHandler<GetByLabelPagedQuery>(repo)
    {
        protected override Task<IReadOnlyList<MailRecipient>> FetchAsync(IMailRecipientRepository repo, GetByLabelPagedQuery query, CancellationToken ct)
            => repo.GetByLabelPagedAsync(query.UserId, query.LabelId, query.Pagination.ToCursor(), query.Pagination.PageSize, ct);
    }
}
