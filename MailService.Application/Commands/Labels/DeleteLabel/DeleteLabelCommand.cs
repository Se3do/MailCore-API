namespace MailService.Application.Commands.Labels.DeleteLabel
{
    public record DeleteLabelCommand(Guid UserId, Guid LabelId);
}
