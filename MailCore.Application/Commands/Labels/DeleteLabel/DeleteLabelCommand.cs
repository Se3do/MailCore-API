using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.DeleteLabel
{
    public record DeleteLabelCommand(Guid UserId, Guid LabelId) : IRequest<bool>, ICommand;
}
