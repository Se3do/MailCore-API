using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Drafts.UpdateDraft
{
    public class UpdateDraftCommandHandler : IRequestHandler<UpdateDraftCommand, bool>
    {
        private readonly IDraftRepository _draftRepository;

        public UpdateDraftCommandHandler(
            IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }

        public async Task<bool> Handle(UpdateDraftCommand command, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct);
            if (draft is null || draft.UserId != command.UserId)
                return false;

            draft.Subject = command.Request.Subject;
            draft.Body = command.Request.Body;
            draft.UpdatedAt = DateTime.UtcNow;

            await _draftRepository.UpdateAsync(command.DraftId, draft, ct);

            return true;
        }
    }

}
