using MailCore.Domain.Common;

namespace MailCore.Application.Common.Pagination
{
    /// <summary>Query parameters for cursor-based pagination.</summary>
    public sealed record CursorPaginationQuery(
        string? EncodedCursor,
        int PageSize = 20)
    {
        public Cursor ToCursor()
            => CursorCodec.Decode(EncodedCursor);
    }
}
