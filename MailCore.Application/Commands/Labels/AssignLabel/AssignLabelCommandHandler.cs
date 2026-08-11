using MailCore.Application.Exceptions;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Labels.AssignLabel
{
    public class AssignLabelCommandHandler : IRequestHandler<AssignLabelCommand>
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public AssignLabelCommandHandler(ILabelRepository labelRepository, IMailRecipientRepository mailRecipientRepository)
        {
            _labelRepository = labelRepository;
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task Handle(AssignLabelCommand command, CancellationToken cancellationToken)
        {
            var label = await _labelRepository.GetByIdAsync(command.LabelId, cancellationToken);
            if (label == null)
                throw new NotFoundException($"Label {command.LabelId} not found.");
            if (label.UserId != command.UserId)
                throw new ForbiddenException("You do not have access to this label.");

            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(command.MailId, cancellationToken);
            if (mailRecipient == null)
                throw new NotFoundException($"Mail {command.MailId} not found.");
            if (mailRecipient.UserId != command.UserId)
                throw new ForbiddenException("You do not have access to this mail.");

            if (mailRecipient.Labels.All(l => l.LabelId != command.LabelId))
            {
                mailRecipient.Labels.Add(MailRecipientLabel.Create(mailRecipient.Id, command.LabelId));
            }
        }
    }
}
