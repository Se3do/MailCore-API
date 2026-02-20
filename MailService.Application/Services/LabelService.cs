using MailService.Application.DTOs.Labels;
using MailService.Application.Services.Interfaces;
using MailService.Application.Queries.Labels.GetAllLabels;
using MailService.Application.Commands.Labels.AssignLabel;
using MailService.Application.Commands.Labels.CreateLabel;
using MailService.Application.Commands.Labels.UpdateLabel;
using MailService.Application.Commands.Labels.DeleteLabel;
using MailService.Application.Commands.Labels.UnassignLabel;

namespace MailService.Application.Services
{
    public class LabelService : ILabelService
    {
        private readonly GetAllLabelsQueryHandler _getAll;
        private readonly CreateLabelCommandHandler _create;
        private readonly UpdateLabelCommandHandler _update;
        private readonly DeleteLabelCommandHandler _delete;
        private readonly AssignLabelCommandHandler _assign;
        private readonly UnassignLabelCommandHandler _unassign;

        public LabelService(
            GetAllLabelsQueryHandler getAll,
            CreateLabelCommandHandler create,
            UpdateLabelCommandHandler update,
            DeleteLabelCommandHandler delete,
            AssignLabelCommandHandler assign,
            UnassignLabelCommandHandler unassign)
        {
            _getAll = getAll;
            _create = create;
            _update = update;
            _delete = delete;
            _assign = assign;
            _unassign = unassign;
        }


        public Task<Guid> CreateAsync(Guid userId, CreateLabelRequest request, CancellationToken ct)
            => _create.Handle(new(userId, request), ct);
        public Task<bool> UpdateAsync(Guid userId, Guid labelId, UpdateLabelRequest request, CancellationToken ct)
            => _update.Handle(new(userId, labelId, request), ct);
        public Task<bool> DeleteAsync(Guid userId, Guid labelId, CancellationToken ct)
            => _delete.Handle(new(userId, labelId), ct);
        public Task<IReadOnlyList<LabelDto>> GetAllAsync(Guid userId, CancellationToken ct)
            => _getAll.Handle(new(userId), ct);

        public Task<bool> AssignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken ct)
            => _assign.Handle(new(userId, mailId, labelId), ct);
        public Task<bool> UnassignLabelAsync(Guid userId, Guid mailId, Guid labelId, CancellationToken ct)
            => _unassign.Handle(new(userId, mailId, labelId), ct);
    }
}
