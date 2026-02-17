namespace MailService.Domain.Common
{
    public sealed record Cursor(DateTime Timestamp, Guid Id)
    {
        public static Cursor Initial =>
            new Cursor(DateTime.MaxValue, Guid.Empty);
    }
}
