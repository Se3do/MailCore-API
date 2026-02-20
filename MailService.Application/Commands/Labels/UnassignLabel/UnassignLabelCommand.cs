using MediatR;

namespace MailService.Application.Commands.Labels.UnassignLabel
{
    public record UnassignLabelCommand(Guid userId, Guid mailId, Guid labelId) : IRequest<bool>;
}
