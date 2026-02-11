using MailService.Domain.Entities;
using MailService.Application.DTOs.Labels;
using MailService.Application.Mappers;
using MailService.Application.Services.Interfaces;
using MailService.Domain.Interfaces;

namespace MailService.Application.Services
{
    public class LabelService : ILabelService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly ILabelRepository labelRepository;
        private readonly IMailRecipientRepository mailRecipientRepository;

        public LabelService(ILabelRepository labelRepository, IUnitOfWork unitOfWork, IMailRecipientRepository mailRecipientRepository)
        {
            this.labelRepository = labelRepository;
            this.unitOfWork = unitOfWork;
            this.mailRecipientRepository = mailRecipientRepository;
        }
        public async Task<LabelDto> CreateAsync(Guid userId, CreateLabelRequest request, CancellationToken cancellationToken = default)
        {
            var label = new Domain.Entities.Label
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = request.Name,
                Color = request.Color ?? string.Empty
            };
            await labelRepository.AddAsync(label, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return label.ToDto();
        }
        public async Task<LabelDto> UpdateAsync(Guid userId, Guid labelId, UpdateLabelRequest request, CancellationToken cancellationToken = default)
        {
            var label = await labelRepository.GetByIdAsync(labelId, cancellationToken);
            if (label == null || label.UserId != userId)
            {
                throw new KeyNotFoundException("Label not found.");
            }

            label.Name = request.Name;
            label.Color = request.Color ?? string.Empty;
            await labelRepository.UpdateAsync(labelId, label, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return label.ToDto();
        }
        public async Task<bool> DeleteAsync(Guid userId, Guid labelId, CancellationToken cancellationToken = default)
        {
            var label = await labelRepository.GetByIdAsync(labelId, cancellationToken);
            if (label == null || label.UserId != userId)
            {
                return false;
            }

            await labelRepository.DeleteAsync(labelId, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<IReadOnlyList<LabelDto>> GetAllAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var labels = await labelRepository.GetAllAsync(userId, cancellationToken);
            return labels.Select(label => label.ToDto()).ToList();
        }

        public async Task<bool> AssignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken cancellationToken = default)
        {
            var label = await labelRepository.GetByIdAsync(labelId, cancellationToken);
            if (label == null || label.UserId != userId)
            {
                return false;
            }

            var mailRecipient = await mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient == null || mailRecipient.UserId != userId)
            {
                return false;
            }

            if (mailRecipient.Labels.Any(l => l.LabelId == labelId))
            {
                return true;
            }

            mailRecipient.Labels.Add(new MailRecipientLabel
            {
                MailRecipientId = mailRecipient.Id,
                LabelId = labelId
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UnassignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken cancellationToken = default)
        {
            var mailRecipient = await mailRecipientRepository.GetByIdAsync(mailId, cancellationToken);
            if (mailRecipient == null || mailRecipient.UserId != userId)
            {
                return false;
            }

            var link = mailRecipient.Labels.FirstOrDefault(l => l.LabelId == labelId);
            if (link == null)
            {
                return true;
            }

            mailRecipient.Labels.Remove(link);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
