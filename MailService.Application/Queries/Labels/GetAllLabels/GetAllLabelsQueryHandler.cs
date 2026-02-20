using MailService.Application.DTOs.Labels;
using MailService.Application.Mappers;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MailService.Application.Queries.Labels.GetAllLabels
{
    public class GetAllLabelsQueryHandler
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
