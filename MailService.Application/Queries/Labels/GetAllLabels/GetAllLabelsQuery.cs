using MailService.Application.DTOs.Labels;
using MediatR;

namespace MailService.Application.Queries.Labels.GetAllLabels
{
    public record GetAllLabelsQuery(Guid UserId): IRequest<IReadOnlyList<LabelDto>>;
}
