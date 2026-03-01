using MailCore.Application.DTOs.Emails;
using MailCore.Application.Mappers;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Email.GetSentById
{
    public class GetSentByIdQueryHandler: IRequestHandler<GetSentByIdQuery, EmailDto?>
    {
        private readonly IEmailRepository _emailRepository;
        public GetSentByIdQueryHandler(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }
        public async Task<EmailDto?> Handle(GetSentByIdQuery query, CancellationToken ct)
        {
            var email = await _emailRepository.GetByIdAsync(query.EmailId, ct);
            if (email == null || email.SenderId != query.UserId)
                return null;
            return email.ToDto();
        }
    }
}
