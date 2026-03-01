using MailCore.Application.DTOs.Drafts;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Drafts.UpdateDraft
{
    public record UpdateDraftCommand(Guid UserId, Guid DraftId, UpdateDraftRequest Request) : IRequest<bool>, ICommand;
}
