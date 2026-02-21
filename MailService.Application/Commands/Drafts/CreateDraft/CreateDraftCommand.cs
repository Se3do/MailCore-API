using MailService.Application.DTOs.Drafts;
using MailService.Domain.Common;
using MediatR;

namespace MailService.Application.Commands.Drafts.CreateDraft
{
    public record CreateDraftCommand(Guid UserId, CreateDraftRequest Request) : IRequest<Guid>, ICommand;
}
