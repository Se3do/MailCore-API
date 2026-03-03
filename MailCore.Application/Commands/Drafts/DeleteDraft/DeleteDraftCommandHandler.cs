using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Drafts.DeleteDraft
{
    public class DeleteDraftCommandHandler : IRequestHandler<DeleteDraftCommand, bool>
    {
        private readonly IDraftRepository _draftRepository;

        public DeleteDraftCommandHandler(IDraftRepository draftRepository) => _draftRepository = draftRepository;

        public async Task<bool> Handle(DeleteDraftCommand command, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(command.DraftId, ct)
                ?? throw new NotFoundException($"Draft {command.DraftId} not found.");

            if (draft.UserId != command.UserId)
                throw new ForbiddenException("You do not have access to this draft.");

            await _draftRepository.DeleteAsync(command.DraftId, ct);
            return true;
        }
    }
}
