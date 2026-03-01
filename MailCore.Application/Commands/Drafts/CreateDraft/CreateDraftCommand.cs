using MailCore.Application.DTOs.Drafts;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Drafts.CreateDraft
{
    public record CreateDraftCommand(Guid UserId, CreateDraftRequest Request) : IRequest<Guid>, ICommand;
}
