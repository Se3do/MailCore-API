using MailCore.Application.Common.Drafts;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Drafts.CreateDraft
{
    public class CreateDraftCommandHandler : IRequestHandler<CreateDraftCommand, Guid>
    {
        private readonly IDraftRepository _draftRepository;

        public CreateDraftCommandHandler(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }

        public async Task<Guid> Handle(CreateDraftCommand command, CancellationToken ct)
        {
            var draft = new Draft
            {
                Id = Guid.NewGuid(),
                UserId = command.UserId,
                ThreadId = command.Request.ThreadId,
                Subject = command.Request.Subject,
                Body = command.Request.Body,
                ToRecipients = DraftRecipientsCodec.Serialize(command.Request.To),
                CcRecipients = DraftRecipientsCodec.Serialize(command.Request.Cc),
                BccRecipients = DraftRecipientsCodec.Serialize(command.Request.Bcc),
                UpdatedAt = DateTime.UtcNow
            };

            await _draftRepository.AddAsync(draft, ct);
            return draft.Id;
        }
    }
}
