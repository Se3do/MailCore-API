using MailCore.Application.Common.Drafts;
using MailCore.Application.DTOs.Drafts;
using MailCore.Domain.Entities;

namespace MailCore.Application.Mappers;

public static class DraftMappings
{
    public static DraftDto ToDto(this Draft draft)
    {
        return new DraftDto(
            draft.Id,
            draft.Subject,
            draft.Body,
            draft.ThreadId,
            DraftRecipientsCodec.Deserialize(draft.ToRecipients),
            DraftRecipientsCodec.Deserialize(draft.CcRecipients),
            DraftRecipientsCodec.Deserialize(draft.BccRecipients),
            new DateTimeOffset(draft.UpdatedAt));
    }
}
