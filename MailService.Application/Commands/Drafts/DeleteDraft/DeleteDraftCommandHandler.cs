using MailService.Domain.Interfaces;

namespace MailService.Application.Commands.Drafts.DeleteDraft
{
    public class DeleteDraftCommandHandler
    {
        private readonly IDraftRepository _draftRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteDraftCommandHandler(
            IDraftRepository draftRepository,
            IUnitOfWork unitOfWork)
        {
            _draftRepository = draftRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteDraftCommand command, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct);
            if (draft is null || draft.UserId != command.UserId)
                return false;

            await _draftRepository.DeleteAsync(command.DraftId, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }
    }

}
