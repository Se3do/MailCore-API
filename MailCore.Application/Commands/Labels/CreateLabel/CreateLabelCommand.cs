using MailCore.Application.DTOs.Labels;
using MailCore.Domain.Common;
using MediatR;

namespace MailCore.Application.Commands.Labels.CreateLabel
{
    public record CreateLabelCommand(Guid userId, CreateLabelRequest request): IRequest<Guid>, ICommand;
}
