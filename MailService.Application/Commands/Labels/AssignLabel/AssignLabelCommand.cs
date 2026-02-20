using MediatR;

namespace MailService.Application.Commands.Labels.AssignLabel
{
    public record AssignLabelCommand(Guid userId, Guid mailId, Guid labelId) : IRequest<bool>;
}
