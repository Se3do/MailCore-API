using MailService.Application.DTOs.Labels;
using MediatR;

namespace MailService.Application.Commands.Labels.UpdateLabel
{
    public record UpdateLabelCommand(Guid userId, Guid labelId, UpdateLabelRequest request) : IRequest<bool>;
}
