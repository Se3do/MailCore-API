using MailService.Application.DTOs.Labels;
using MailService.Application.Mappers;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Queries.Labels.GetAllLabels
{
    public class GetAllLabelsQueryHandler: IRequestHandler<GetAllLabelsQuery, IReadOnlyList<LabelDto>>
    {
        private readonly ILabelRepository _labelRepository;

        public GetAllLabelsQueryHandler(ILabelRepository labelRepository)
        {
            _labelRepository = labelRepository;
        }

        public async Task<IReadOnlyList<LabelDto>> Handle(GetAllLabelsQuery query, CancellationToken ct)
        {
            var userId = query.UserId;
            var labels = await _labelRepository.GetAllAsync(userId, ct);
            return labels.Select(label => label.ToDto()).ToList();
        }

    }
}
