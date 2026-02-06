using MailService.Application.DTOs.Labels;
using MailService.Domain.Entities;

namespace MailService.Application.Mappers;

public static class LabelMappings
{
    public static LabelDto ToDto(this Label label)
    {
        return new LabelDto(
            label.Id,
            label.Name,
            label.Color);
    }
}
