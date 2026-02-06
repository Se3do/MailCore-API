using MailService.Application.DTOs.Drafts;
using MailService.Domain.Entities;

namespace MailService.Application.Mappers;

public static class DraftMappings
{
    public static DraftDto ToDto(this Draft draft)
    {
        return new DraftDto(
            draft.Id,
            draft.Subject,
            draft.Body,
            Array.Empty<string>(),
            null,
            null,
            draft.ThreadId,
            new DateTimeOffset(draft.UpdatedAt));
    }
}
