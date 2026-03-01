using MailCore.Application.DTOs.Drafts;
using MediatR;

namespace MailCore.Application.Queries.Drafts.GetDraftById
{
    public record GetDraftByIdQuery(Guid UserId, Guid DraftId): IRequest<DraftDto>;
}
