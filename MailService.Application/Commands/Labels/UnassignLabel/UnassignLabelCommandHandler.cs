using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailService.Application.Commands.Labels.UnassignLabel
{
    public class UnassignLabelCommandHandler
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UnassignLabelCommandHandler(IMailRecipientRepository mailRecipientRepository, IUnitOfWork unitOfWork)
        {
            _mailRecipientRepository = mailRecipientRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UnassignLabelCommand command, CancellationToken ct)
        {
            var userId = command.userId;
            var mailId = command.mailId;
            var labelId = command.labelId;

            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(mailId, ct);
            if (mailRecipient == null || mailRecipient.UserId != userId)
            {
                return false;
            }

            var link = mailRecipient.Labels.FirstOrDefault(l => l.LabelId == labelId);
            if (link == null)
            {
                return true;
            }

            mailRecipient.Labels.Remove(link);
            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

    }
}
