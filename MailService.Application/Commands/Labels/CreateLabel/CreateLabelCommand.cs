using MailService.Application.DTOs.Labels;
using MediatR;

namespace MailService.Application.Commands.Labels.CreateLabel
{
    public record CreateLabelCommand(Guid userId, CreateLabelRequest request): IRequest<Guid>;
}
