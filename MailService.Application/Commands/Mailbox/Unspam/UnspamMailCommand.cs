namespace MailService.Application.Commands.Mailbox.Unspam
{
    public sealed record UnspamMailCommand(Guid UserId, Guid MailId);
}
