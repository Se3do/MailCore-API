namespace MailService.Application.Queries.Mailbox.GetMailById
{
    public sealed record GetMailByIdQuery(Guid UserId, Guid MailId);
}