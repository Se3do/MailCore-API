using MailService.Domain.Entities;
using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Drafts.CreateDraft
{
    public class CreateDraftCommandHandler
    {
        private readonly IDraftRepository _draftRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateDraftCommandHandler(IDraftRepository draftRepository, IUnitOfWork unitOfWork)
        {
            _draftRepository = draftRepository;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync(ct);

            return draft.Id;
        }
    }

}
