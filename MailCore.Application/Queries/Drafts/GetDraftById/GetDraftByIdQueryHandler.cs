using MailCore.Application.DTOs.Drafts;
using MailCore.Application.Exceptions;
using MailCore.Application.Mappers;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Drafts.GetDraftById
{
    public class GetDraftByIdQueryHandler: IRequestHandler<GetDraftByIdQuery, DraftDto>
    {
        private readonly IDraftRepository _draftRepository;

        public GetDraftByIdQueryHandler(IDraftRepository draftRepository)
        {
            _draftRepository = draftRepository;
        }

        public async Task<DraftDto> Handle(GetDraftByIdQuery query, CancellationToken ct)
        {
            var draft = await _draftRepository.GetByIdAsync(query.DraftId, ct)
                ?? throw new NotFoundException($"Draft {query.DraftId} not found.");

            if (draft.UserId != query.UserId)
                throw new ForbiddenException("You do not have access to this draft.");

            return draft.ToDto();
        }
    }

}
