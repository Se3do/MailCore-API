using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.AssignLabel
{
    public record AssignLabelCommand(Guid UserId, Guid MailId, Guid LabelId) : IRequest<bool>, ICommand;
}
