using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Labels.DeleteLabel
{
    public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, bool>
    {
        private readonly ILabelRepository _labelRepository;

        public DeleteLabelCommandHandler(ILabelRepository labelRepository)
        {
            this._labelRepository = labelRepository;
        }

        public async Task<bool> Handle(DeleteLabelCommand command, CancellationToken ct)
        {
            var userId = command.UserId;
            var labelId = command.LabelId;
            var label = await _labelRepository.GetByIdAsync(labelId, ct);
            if (label == null || label.UserId != userId)
            {
                return false;
            }

            await _labelRepository.DeleteAsync(labelId, ct);

            return true;
        }
    }
}
