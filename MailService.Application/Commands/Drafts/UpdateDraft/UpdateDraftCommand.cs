using MailService.Application.DTOs.Drafts;
using MediatR;

namespace MailService.Application.Commands.Drafts.UpdateDraft
{
    public record UpdateDraftCommand(Guid UserId, Guid DraftId, UpdateDraftRequest Request) : IRequest<bool>;
}
