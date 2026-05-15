using MailCore.Application.DTOs.Attachments;
using MailCore.Application.DTOs.Emails;
using MailCore.Application.DTOs.Mailbox;
using MailCore.Application.Mappers;
using MailCore.Domain.Entities;
using MailCore.Domain.Enums;

namespace MailCore.Application.Tests.Mappers;

public class MapperTests
{
    private static void SetField<T>(T target, string name, object? value)
    {
        var field = typeof(T).GetField($"<{name}>k__BackingField",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        field!.SetValue(target, value);
    }

    [Fact]
    public void ToSummaryDto_MapsAllFields()
    {
        var sender = User.Create("Alice", "alice@test.com", "hash");
        var email = Email.Create(sender.Id, "Subject", "Body text",
            createdAt: new DateTime(2025, 1, 1));
        SetField(email, nameof(Email.Sender), sender);

        var dto = email.ToSummaryDto();

        Assert.Equal(email.Id, dto.Id);
        Assert.Equal("Subject", dto.Subject);
        Assert.Equal("Body text", dto.Preview);
        Assert.Equal("alice@test.com", dto.From);
        Assert.Equal(new DateTimeOffset(email.CreatedAt), dto.SentAt);
        Assert.Equal(email.ThreadId, dto.ThreadId);
        Assert.False(dto.HasAttachments);
    }

    [Fact]
    public void ToSummaryDto_SenderIsNull_UsesEmptyFrom()
    {
        var email = Email.Create(Guid.NewGuid(), "Sub", "Body");
        SetField(email, nameof(Email.Sender), null);

        var dto = email.ToSummaryDto();

        Assert.Equal(string.Empty, dto.From);
    }

    [Fact]
    public void ToSummaryDto_LongBody_TruncatesPreview()
    {
        var sender = User.Create("Bob", "b@t.com", "h");
        var longBody = new string('x', 200);
        var email = Email.Create(sender.Id, "Sub", longBody);
        SetField(email, nameof(Email.Sender), sender);

        var dto = email.ToSummaryDto();

        Assert.Equal(100, dto.Preview.Length);
        Assert.Equal(new string('x', 100), dto.Preview);
    }

    [Fact]
    public void ToSummaryDto_NullBody_CreatesEmptyPreview()
    {
        var sender = User.Create("C", "c@t.com", "h");
        var email = Email.Create(sender.Id, "Sub", null!);
        SetField(email, nameof(Email.Sender), sender);

        var dto = email.ToSummaryDto();

        Assert.Equal(string.Empty, dto.Preview);
    }

    [Fact]
    public void ToSummaryDto_EmptyBody_CreatesEmptyPreview()
    {
        var sender = User.Create("D", "d@t.com", "h");
        var email = Email.Create(sender.Id, "Sub", "");
        SetField(email, nameof(Email.Sender), sender);

        var dto = email.ToSummaryDto();

        Assert.Equal(string.Empty, dto.Preview);
    }

    [Fact]
    public void ToDto_MapsAllFields()
    {
        var sender = User.Create("Eve", "eve@t.com", "h");
        var email = Email.Create(sender.Id, "Full", "Full body",
            createdAt: new DateTime(2025, 3, 1));
        SetField(email, nameof(Email.Sender), sender);

        var toR = MailRecipient.Create(Guid.NewGuid(), email.Id, RecipientType.To, DateTime.UtcNow);
        var toU = User.Create("To", "to@t.com", "h");
        SetField(toR, nameof(MailRecipient.User), toU);

        var ccR = MailRecipient.Create(Guid.NewGuid(), email.Id, RecipientType.Cc, DateTime.UtcNow);
        var ccU = User.Create("Cc", "cc@t.com", "h");
        SetField(ccR, nameof(MailRecipient.User), ccU);

        var bccR = MailRecipient.Create(Guid.NewGuid(), email.Id, RecipientType.Bcc, DateTime.UtcNow);
        var bccU = User.Create("Bcc", "bcc@t.com", "h");
        SetField(bccR, nameof(MailRecipient.User), bccU);

        var att = Attachment.Create(email.Id, "f.pdf", "application/pdf", 1024, "k");
        SetField(email, nameof(Email.Recipients), new List<MailRecipient> { toR, ccR, bccR });
        SetField(email, nameof(Email.Attachments), new List<Attachment> { att });

        var dto = email.ToDto();

        Assert.Equal(email.Id, dto.Id);
        Assert.Equal("Full", dto.Subject);
        Assert.Equal("Full body", dto.Body);
        Assert.Equal("eve@t.com", dto.From);
        Assert.Equal(new DateTimeOffset(email.CreatedAt), dto.SentAt);
        Assert.Equal(email.ThreadId, dto.ThreadId);
        Assert.Single(dto.To);
        Assert.Contains("to@t.com", dto.To);
        Assert.NotNull(dto.Cc);
        Assert.Contains("cc@t.com", dto.Cc);
        Assert.NotNull(dto.Bcc);
        Assert.Contains("bcc@t.com", dto.Bcc);
        Assert.Single(dto.Attachments);
        Assert.Equal("f.pdf", dto.Attachments[0].FileName);
    }

    [Fact]
    public void ToDto_NoRecipients_ReturnsEmptyTo()
    {
        var sender = User.Create("F", "f@t.com", "h");
        var email = Email.Create(sender.Id, "Sub", "Body");
        SetField(email, nameof(Email.Sender), sender);

        var dto = email.ToDto();

        Assert.Empty(dto.To);
        Assert.Null(dto.Cc);
        Assert.Null(dto.Bcc);
        Assert.Empty(dto.Attachments);
    }

    [Fact]
    public void ToMailboxItemDto_MapsAllFields()
    {
        var sender = User.Create("Grace", "grace@t.com", "h");
        var email = Email.Create(sender.Id, "Hello", "Long body content",
            createdAt: new DateTime(2025, 6, 15));
        SetField(email, nameof(Email.Sender), sender);

        var recipient = MailRecipient.Create(sender.Id, email.Id, RecipientType.To, DateTime.UtcNow);
        SetField(recipient, nameof(MailRecipient.Email), email);
        recipient.MarkAsRead();
        recipient.MarkAsStarred();

        var dto = recipient.ToMailboxItemDto();

        Assert.Equal(recipient.Id, dto.MailRecipientId);
        Assert.Equal(email.Id, dto.EmailId);
        Assert.Equal("grace@t.com", dto.From);
        Assert.Equal("Hello", dto.Subject);
        Assert.Equal("Long body content", dto.Preview);
        Assert.Equal(new DateTimeOffset(email.CreatedAt), dto.SentAt);
        Assert.True(dto.IsRead);
        Assert.True(dto.IsStarred);
        Assert.False(dto.IsSpam);
        Assert.False(dto.IsTrash);
        Assert.Equal(email.ThreadId, dto.ThreadId);
    }

    [Fact]
    public void ToMailboxItemDto_DeletedRecipient_IsTrashTrue()
    {
        var sender = User.Create("H", "h@t.com", "h");
        var email = Email.Create(sender.Id, "Sub", "Body");
        SetField(email, nameof(Email.Sender), sender);

        var recipient = MailRecipient.Create(sender.Id, email.Id, RecipientType.To, DateTime.UtcNow);
        SetField(recipient, nameof(MailRecipient.Email), email);
        recipient.SoftDelete();

        var dto = recipient.ToMailboxItemDto();

        Assert.True(dto.IsTrash);
    }

    [Fact]
    public void ToMailboxDetailDto_MapsLabelsAndEmailDto()
    {
        var sender = User.Create("Ivy", "ivy@t.com", "h");
        var email = Email.Create(sender.Id, "Detail", "Detail body",
            createdAt: new DateTime(2025, 7, 1));
        SetField(email, nameof(Email.Sender), sender);

        var toR = MailRecipient.Create(Guid.NewGuid(), email.Id, RecipientType.To, DateTime.UtcNow);
        var toU = User.Create("To", "to@t.com", "h");
        SetField(toR, nameof(MailRecipient.User), toU);
        SetField(toR, nameof(MailRecipient.Email), email);

        SetField(email, nameof(Email.Recipients), new List<MailRecipient> { toR });

        var label = Label.Create(Guid.NewGuid(), "Work", "#blue");
        var mrl = MailRecipientLabel.Create(Guid.NewGuid(), label.Id);
        SetField(mrl, nameof(MailRecipientLabel.Label), label);

        var recipient = MailRecipient.Create(sender.Id, email.Id, RecipientType.To, DateTime.UtcNow);
        SetField(recipient, nameof(MailRecipient.Email), email);
        SetField(recipient, nameof(MailRecipient.Labels), new List<MailRecipientLabel> { mrl });

        var dto = recipient.ToMailboxDetailDto();

        Assert.Equal(recipient.Id, dto.MailRecipientId);
        Assert.NotNull(dto.Email);
        Assert.Equal(email.Id, dto.Email.Id);
        Assert.Equal("Detail", dto.Email.Subject);
        Assert.Single(dto.Labels);
        Assert.Equal("Work", dto.Labels[0].Name);
        Assert.Equal("#blue", dto.Labels[0].Color);
    }

    [Fact]
    public void ToMailboxDetailDto_MapsRecipientFlags()
    {
        var sender = User.Create("J", "j@t.com", "h");
        var email = Email.Create(sender.Id, "Sub", "Body");
        SetField(email, nameof(Email.Sender), sender);

        var recipient = MailRecipient.Create(sender.Id, email.Id, RecipientType.To, DateTime.UtcNow);
        SetField(recipient, nameof(MailRecipient.Email), email);
        recipient.MarkAsRead();
        recipient.MarkAsSpam();
        recipient.SoftDelete();

        var dto = recipient.ToMailboxDetailDto();

        Assert.True(dto.IsRead);
        Assert.True(dto.IsSpam);
        Assert.True(dto.IsTrash);
    }
}
