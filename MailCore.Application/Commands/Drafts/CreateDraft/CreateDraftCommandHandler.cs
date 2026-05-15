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
            var draft = Draft.Create(
                command.UserId,
                command.Request.Subject,
                command.Request.Body,
                DraftRecipientsCodec.Serialize(command.Request.To),
                DraftRecipientsCodec.Serialize(command.Request.Cc),
                DraftRecipientsCodec.Serialize(command.Request.Bcc),
                command.Request.ThreadId);

            await _draftRepository.AddAsync(draft, ct);
            return draft.Id;
        }
    }
}
