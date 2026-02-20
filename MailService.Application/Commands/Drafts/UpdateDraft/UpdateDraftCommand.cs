using MailService.Application.DTOs.Drafts;

namespace MailService.Application.Commands.Drafts.UpdateDraft
{
    public record UpdateDraftCommand(Guid UserId, Guid DraftId, UpdateDraftRequest Request);
}
