namespace MailService.Application.Queries.Email.GetSentById
{
    public record GetSentByIdQuery(Guid UserId, Guid EmailId);
}
