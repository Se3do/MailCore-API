using MailService.Application.DTOs.Emails;
using MailService.Application.Mappers;
using MailService.Domain.Interfaces;
using MediatR;

namespace MailService.Application.Queries.Email.GetSentById
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
