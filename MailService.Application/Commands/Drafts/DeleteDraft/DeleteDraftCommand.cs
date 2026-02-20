using MediatR;

namespace MailService.Application.Commands.Drafts.DeleteDraft
{
    public record DeleteDraftCommand(Guid UserId, Guid DraftId) : IRequest<bool>;
}
