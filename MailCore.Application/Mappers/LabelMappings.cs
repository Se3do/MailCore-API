using MailCore.Application.DTOs.Labels;
using MailCore.Domain.Entities;

namespace MailCore.Application.Mappers;

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
