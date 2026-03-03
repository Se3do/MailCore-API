using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Drafts.SendDraft;

public record SendDraftCommand(Guid UserId, Guid DraftId) : IRequest, ICommand;
