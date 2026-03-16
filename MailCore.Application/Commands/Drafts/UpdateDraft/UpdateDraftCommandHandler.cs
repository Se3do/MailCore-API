using MailCore.Application.Common.Drafts;
using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Drafts.UpdateDraft
{
    public class UpdateDraftCommandHandler : IRequestHandler<UpdateDraftCommand, bool>
    {
        private readonly IDraftRepository _draftRepository;

        public UpdateDraftCommandHandler(IDraftRepository draftRepository) => _draftRepository = draftRepository;

        public async Task<bool> Handle(UpdateDraftCommand command, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct)
                 ?? throw new NotFoundException($"Draft {command.DraftId} not found.");

            if (draft.UserId != command.UserId)
                 throw new ForbiddenException("You do not have access to this draft.");

            draft.Subject = command.Request.Subject;
            draft.Body = command.Request.Body;
            draft.ToRecipients = DraftRecipientsCodec.Serialize(command.Request.To);
            draft.CcRecipients = DraftRecipientsCodec.Serialize(command.Request.Cc);
            draft.BccRecipients = DraftRecipientsCodec.Serialize(command.Request.Bcc);
            draft.UpdatedAt = DateTime.UtcNow;

            await _draftRepository.UpdateAsync(command.DraftId, draft, ct);
            return true;
        }
    }
}
