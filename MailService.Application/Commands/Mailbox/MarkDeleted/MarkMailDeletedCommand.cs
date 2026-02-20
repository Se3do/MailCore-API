namespace MailService.Application.Commands.Mailbox.MarkDeleted
{
 public sealed record MarkMailDeletedCommand(Guid UserId, Guid MailId);
}
