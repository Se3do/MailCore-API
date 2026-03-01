using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.AssignLabel
{
    public record AssignLabelCommand(Guid userId, Guid mailId, Guid labelId) : IRequest<bool>, ICommand;
}
