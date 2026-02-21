using MailService.Domain.Common;
using MediatR;

namespace MailService.Application.Commands.Labels.DeleteLabel
{
    public record DeleteLabelCommand(Guid UserId, Guid LabelId) : IRequest<bool>, ICommand;
}
