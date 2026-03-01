using MailCore.Application.DTOs.Labels;
using MediatR;

namespace MailCore.Application.Queries.Labels.GetAllLabels
{
    public record GetAllLabelsQuery(Guid UserId): IRequest<IReadOnlyList<LabelDto>>;
}
