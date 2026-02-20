using MailService.Application.DTOs.Drafts;
using MailService.Application.Mappers;
using MailService.Domain.Entities;
using MailService.Domain.Interfaces;

namespace MailService.Application.Queries.Drafts.GetDraftById
{
    public class GetDraftByIdQueryHandler
    {
        private readonly IDraftRepository _draftRepository;

        public GetDraftByIdQueryHandler(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }

        public async Task<DraftDto?> Handle(GetDraftByIdQuery query, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(query.DraftId, ct);

            if (draft == null || draft.UserId != query.UserId)
            {
                return null;
            }
            return draft.ToDto();
        }
    }

}
