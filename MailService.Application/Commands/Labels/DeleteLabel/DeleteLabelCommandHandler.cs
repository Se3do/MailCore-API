using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Commands.Labels.DeleteLabel
{
    public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, bool>
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLabelCommandHandler(ILabelRepository labelRepository, IUnitOfWork unitOfWork)
        {
            this._labelRepository = labelRepository;
            this._unitOfWork = unitOfWork;
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
            await _unitOfWork.SaveChangesAsync(ct);

            return true;
        }
    }
}
