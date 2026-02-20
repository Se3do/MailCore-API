using MailService.Application.DTOs.Labels;

namespace MailService.Application.Commands.Labels.UpdateLabel
{
    public record UpdateLabelCommand(Guid userId, Guid labelId, UpdateLabelRequest request);
}
