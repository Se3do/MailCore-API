using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Drafts.DeleteDraft
{
    public record DeleteDraftCommand(Guid UserId, Guid DraftId) : IRequest<bool>, ICommand;
}
