using MailService.Application.Services.Interfaces;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailService.Application.Commands.Labels.AssignLabel
{
    public class AssignLabelCommandHandler
    {
        private readonly ILabelRepository _labelRepository;
        private readonly IMailRecipientRepository _mailRecipientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AssignLabelCommandHandler(ILabelRepository labelRepository, IMailRecipientRepository mailRecipientRepository, IUnitOfWork unitOfWork)
        {
            _labelRepository = labelRepository;
            _mailRecipientRepository = mailRecipientRepository;
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
