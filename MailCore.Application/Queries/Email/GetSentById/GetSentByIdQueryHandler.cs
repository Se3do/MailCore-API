using MailCore.Application.DTOs.Emails;
using MailCore.Application.Exceptions;
using MailCore.Application.Mappers;
using MailCore.Domain.Interfaces;
using MediatR;

namespace MailCore.Application.Queries.Email.GetSentById
{
    public class GetSentByIdQueryHandler : IRequestHandler<GetSentByIdQuery, EmailDto?>
    {
        private readonly IEmailRepository _emailRepository;

        public GetSentByIdQueryHandler(IEmailRepository emailRepository)
        {
            _emailRepository = emailRepository;
        }

        public async Task<EmailDto?> Handle(GetSentByIdQuery query, CancellationToken ct)
        {
            var email = await _emailRepository.GetByIdAsync(query.EmailId, ct)
                ?? throw new NotFoundException($"Email {query.EmailId} not found.");

            if (email.SenderId != query.UserId)
                throw new ForbiddenException("You do not have access to this email.");

            return email.ToDto();
        }
    }
}
