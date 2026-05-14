namespace MailCore.Infrastructure.MailSender
{
    public class SmtpOptions
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 1025;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string FromAddress { get; set; } = "noreply@mailcore.local";
        public string FromName { get; set; } = "MailCore";
        public bool UseSsl { get; set; }
    }
}
