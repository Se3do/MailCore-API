using MailService.Application.DTOs.Labels;

namespace MailService.Application.Commands.Labels.CreateLabel
{
    public record CreateLabelCommand(Guid userId, CreateLabelRequest request);
}
