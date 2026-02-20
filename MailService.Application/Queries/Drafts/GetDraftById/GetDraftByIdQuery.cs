using MailService.Application.DTOs.Drafts;
using MediatR;

namespace MailService.Application.Queries.Drafts.GetDraftById
{
    public record GetDraftByIdQuery(Guid UserId, Guid DraftId): IRequest<DraftDto>;
}
