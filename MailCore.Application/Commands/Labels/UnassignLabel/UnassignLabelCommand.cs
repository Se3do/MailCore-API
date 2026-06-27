using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.UnassignLabel
{
    public record UnassignLabelCommand(Guid UserId, Guid MailId, Guid LabelId) : IRequest<bool>, ICommand;
}
