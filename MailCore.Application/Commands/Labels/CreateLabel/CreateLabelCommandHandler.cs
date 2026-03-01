using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Labels.CreateLabel
{
    public class CreateLabelCommandHandler : IRequestHandler<CreateLabelCommand, Guid>
    {
        private readonly ILabelRepository _labelRepository;

        public CreateLabelCommandHandler(ILabelRepository labelRepository)
        {
            this._labelRepository = labelRepository;
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
            return label.Id;
        }
    }
}
