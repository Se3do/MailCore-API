using MailCore.Application.DTOs.Drafts;
using MailCore.Application.Mappers;
using MailCore.Domain.Entities;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Drafts.GetDraftById
{
    public class GetDraftByIdQueryHandler: IRequestHandler<GetDraftByIdQuery, DraftDto?>
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
