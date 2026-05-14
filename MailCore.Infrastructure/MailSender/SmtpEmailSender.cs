using MailCore.Application.Interfaces.Persistence;
using MailCore.Application.Interfaces.Services;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace MailCore.Infrastructure.MailSender
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _options;
        private readonly IFileStorage _fileStorage;

        public SmtpEmailSender(IOptions<SmtpOptions> options, IFileStorage fileStorage)
        {
            _options = options.Value;
            _fileStorage = fileStorage;
        }

        public async Task SendAsync(Domain.Entities.Email email, IReadOnlyList<MailRecipient> recipients, CancellationToken ct = default)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
            message.Subject = email.Subject;

            var body = new TextPart("plain")
            {
                Text = email.Body
            };

            if (email.HasAttachments && email.Attachments.Count > 0)
            {
                var multipart = new Multipart("mixed");
                multipart.Add(body);

                foreach (var attachment in email.Attachments)
                {
                    var stream = await _fileStorage.GetAsync(attachment.StorageKey, ct);
                    var mimePart = new MimePart(attachment.ContentType)
                    {
                        Content = new MimeContent(stream),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = attachment.FileName
                    };
                    multipart.Add(mimePart);
                }

                message.Body = multipart;
            }
            else
            {
                message.Body = body;
            }

            foreach (var recipient in recipients)
            {
                if (recipient.User?.Email is null) continue;
                var mailbox = new MailboxAddress(recipient.User.Name, recipient.User.Email);
                switch (recipient.Type)
                {
                    case RecipientType.To:
                        message.To.Add(mailbox);
                        break;
                    case RecipientType.Cc:
                        message.Cc.Add(mailbox);
                        break;
                    case RecipientType.Bcc:
                        message.Bcc.Add(mailbox);
                        break;
                }
            }

            using var client = new SmtpClient();

            if (!_options.UseSsl)
            {
                await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable, ct);
            }
            else
            {
                await client.ConnectAsync(_options.Host, _options.Port, _options.UseSsl, ct);
            }

            if (!string.IsNullOrEmpty(_options.Username) && !string.IsNullOrEmpty(_options.Password))
            {
                await client.AuthenticateAsync(_options.Username, _options.Password, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
