using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MailCore.Application.Interfaces.Services;
using MediatR;
using System.Threading;

namespace MailCore.Application.Commands.Labels.AssignLabel
{
    public class AssignLabelCommandHandler : IRequestHandler<AssignLabelCommand, bool>
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public AssignLabelCommandHandler(ILabelRepository labelRepository, IMailRecipientRepository mailRecipientRepository)
        {
            _labelRepository = labelRepository;
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task<bool> Handle(AssignLabelCommand command, CancellationToken cancellationToken)
        {
            var userId = command.userId;
            var mailId = command.mailId;
            var labelId = command.labelId;

            var label = await _labelRepository.GetByIdAsync(labelId, cancellationToken);
            if (label == null || label.UserId != userId)
            {
                return false;
            }

            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient == null || mailRecipient.UserId != userId)
            {
                return false;
            }

            if (mailRecipient.Labels.Any(l => l.LabelId == labelId))
            {
                return true;
            }

            mailRecipient.Labels.Add(new MailRecipientLabel
            {
                MailRecipientId = mailRecipient.Id,
                LabelId = labelId
            });

            return true;
        }
    }
}
