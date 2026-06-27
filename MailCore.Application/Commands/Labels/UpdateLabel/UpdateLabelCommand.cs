using MailCore.Application.DTOs.Labels;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.UpdateLabel
{
    public record UpdateLabelCommand(Guid UserId, Guid LabelId, UpdateLabelRequest Request) : IRequest<bool>, ICommand;
}
