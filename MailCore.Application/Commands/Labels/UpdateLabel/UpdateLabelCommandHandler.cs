using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;
using System.Threading;

namespace MailCore.Application.Commands.Labels.UpdateLabel
{
    public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, bool>
    {
        private readonly ILabelRepository _labelRepository;

        public UpdateLabelCommandHandler(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }
        public async Task<bool> Handle(UpdateLabelCommand command, CancellationToken ct)
        {
            var userId = command.userId;
            var labelId = command.labelId;
            var request = command.request;

            var label = await _labelRepository.GetByIdAsync(labelId, ct);
            if (label == null || label.UserId != userId)
                return false;

            label.Name = request.Name;
            label.Color = request.Color ?? string.Empty;

            await _labelRepository.UpdateAsync(labelId, label, ct);

            return true;
        }
    }
}
