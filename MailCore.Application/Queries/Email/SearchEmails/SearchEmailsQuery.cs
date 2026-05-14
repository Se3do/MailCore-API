using MailCore.Application.Common.Pagination;
using MailCore.Application.DTOs.Emails;
using MediatR;

namespace MailCore.Application.Queries.Email.SearchEmails;

public record SearchEmailsQuery(Guid UserId, string Query, CursorPaginationQuery Pagination) : IRequest<CursorPagedResult<EmailSummaryDto>>;
