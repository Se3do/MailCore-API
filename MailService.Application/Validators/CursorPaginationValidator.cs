using FluentValidation;
using MailService.Application.Common.Pagination;

namespace MailService.Application.Validators
{
    public sealed class CursorPaginationValidator
        : AbstractValidator<CursorPaginationQuery>
    {
        public CursorPaginationValidator()
        {
            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(50)
                .WithMessage("PageSize must be between 1 and 50.");
        }
    }
}
