using MailCore.Application.DTOs.Labels;
using MailCore.Application.Mappers;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Labels.GetAllLabels
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
