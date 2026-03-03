using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Labels.DeleteLabel
{
    public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, bool>
    {
        private readonly ILabelRepository _labelRepository;

        public DeleteLabelCommandHandler(ILabelRepository labelRepository) => _labelRepository = labelRepository;

        public async Task<bool> Handle(DeleteLabelCommand command, CancellationToken ct)
        {
            var label = await _labelRepository.GetByIdAsync(command.LabelId, ct)
                ?? throw new NotFoundException($"Label {command.LabelId} not found.");

            if (label.UserId != command.UserId)
                throw new ForbiddenException("You do not have access to this label.");

            await _labelRepository.DeleteAsync(command.LabelId, ct);
            return true;
        }
    }
}
