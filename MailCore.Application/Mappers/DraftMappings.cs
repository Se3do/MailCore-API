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
            new DateTimeOffset(draft.UpdatedAt));
    }
}
