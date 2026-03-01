using MailCore.Application.DTOs.Labels;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.UpdateLabel
{
    public record UpdateLabelCommand(Guid userId, Guid labelId, UpdateLabelRequest request) : IRequest<bool>, ICommand;
}
