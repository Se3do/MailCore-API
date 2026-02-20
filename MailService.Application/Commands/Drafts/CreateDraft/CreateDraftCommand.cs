using MailService.Application.DTOs.Drafts;

namespace MailService.Application.Commands.Drafts.CreateDraft
{
    public record CreateDraftCommand(Guid UserId, CreateDraftRequest Request
    );
}
