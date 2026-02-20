using MailService.Application.Mappers;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using MediatR;
using System.Reflection.Metadata;
using System.Threading;

namespace MailService.Application.Commands.Labels.CreateLabel
{
    public class CreateLabelCommandHandler
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateLabelCommandHandler(ILabelRepository labelRepository, IUnitOfWork unitOfWork)
        {
            this._labelRepository = labelRepository;
            this._unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateLabelCommand command, CancellationToken ct)
        {
            var userId = command.userId;
            var request = command.request;
            var label = new Domain.Entities.Label
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name,
                Color = request.Color ?? string.Empty
            };
            await _labelRepository.AddAsync(label, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return label.Id;
        }
    }
}
