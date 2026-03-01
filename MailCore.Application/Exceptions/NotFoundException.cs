namespace MailCore.Application.Exceptions
{
    public sealed class NotFoundException: AppException
    {
        public NotFoundException(string message) : base(message) { }
    }
}
