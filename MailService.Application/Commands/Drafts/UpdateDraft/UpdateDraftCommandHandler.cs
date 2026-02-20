using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Drafts.UpdateDraft
{
    public class UpdateDraftCommandHandler
    {
        private readonly IDraftRepository _draftRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateDraftCommandHandler(
            IDraftRepository draftRepository,
            IUnitOfWork unitOfWork)
        {
            _draftRepository = draftRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateDraftCommand command, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct);
            if (draft is null || draft.UserId != command.UserId)
                return false;

            draft.Subject = command.Request.Subject;
            draft.Body = command.Request.Body;
            draft.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }

}
