using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;
using System.Threading;

namespace MailCore.Application.Commands.Labels.UpdateLabel
{
    public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, bool>
    {
        private readonly ILabelRepository _labelRepository;

        public UpdateLabelCommandHandler(ILabelRepository labelRepository)
            => _labelRepository = labelRepository;

        public async Task<bool> Handle(UpdateLabelCommand command, CancellationToken ct)
        {
            var label = await _labelRepository.GetByIdAsync(command.labelId, ct)
                ?? throw new NotFoundException($"Label {command.labelId} not found.");

            if (label.UserId != command.userId)
                throw new ForbiddenException("You do not have access to this label.");

            label.Name = command.request.Name;
            label.Color = command.request.Color ?? string.Empty;

            await _labelRepository.UpdateAsync(command.labelId, label, ct);

            return true;
        }
    }
}
