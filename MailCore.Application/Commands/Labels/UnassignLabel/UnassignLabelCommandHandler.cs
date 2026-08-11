using MailCore.Application.Exceptions;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Commands.Labels.UnassignLabel
{
    public class UnassignLabelCommandHandler : IRequestHandler<UnassignLabelCommand>
    {
        private readonly IMailRecipientRepository _mailRecipientRepository;

        public UnassignLabelCommandHandler(IMailRecipientRepository mailRecipientRepository)
        {
            _mailRecipientRepository = mailRecipientRepository;
        }

        public async Task Handle(UnassignLabelCommand command, CancellationToken ct)
        {
            var mailRecipient = await _mailRecipientRepository.GetByIdAsync(command.MailId, ct);
            if (mailRecipient == null)
                throw new NotFoundException($"Mail {command.MailId} not found.");
            if (mailRecipient.UserId != command.UserId)
                throw new ForbiddenException("You do not have access to this mail.");

            var link = mailRecipient.Labels.FirstOrDefault(l => l.LabelId == command.LabelId);
            if (link != null)
            {
                mailRecipient.Labels.Remove(link);
            }
        }
    }
}
