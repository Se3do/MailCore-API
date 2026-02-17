using MailService.Domain.Common;

namespace MailService.Application.Common.Pagination
{
    public sealed record CursorPaginationQuery(
        string? EncodedCursor,
        int PageSize = 20)
    {
        public Cursor ToCursor()
            => CursorCodec.Decode(EncodedCursor);
    }
}
