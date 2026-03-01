using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.UnassignLabel
{
    public record UnassignLabelCommand(Guid userId, Guid mailId, Guid labelId) : IRequest<bool>, ICommand;
}
