using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MediatR;
using System.Threading;

namespace MailService.Application.Commands.Labels.UpdateLabel
{
    public class UpdateLabelCommandHandler
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateLabelCommandHandler(ILabelRepository labelRepository, IUnitOfWork unitOfWork)
        {
            _labelRepository = labelRepository;
            _unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }
    }
}
