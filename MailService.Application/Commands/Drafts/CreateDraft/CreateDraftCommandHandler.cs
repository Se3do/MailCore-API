using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Drafts.CreateDraft
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
                Subject = command.Request.Subject,
                Body = command.Request.Body,
                UpdatedAt = DateTime.UtcNow
            };

            await _draftRepository.AddAsync(draft, ct);
            return draft.Id;
        }
    }
}
