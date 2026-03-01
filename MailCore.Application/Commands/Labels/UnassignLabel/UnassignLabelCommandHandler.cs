using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Labels.UnassignLabel
{
    public class UnassignLabelCommandHandler : IRequestHandler<UnassignLabelCommand, bool>
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public UnassignLabelCommandHandler(IMailRecipientRepository mailRecipientRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
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
            return true;
        }
    }
}
