using MailCore.Domain.Common;

namespace MailCore.Application.Common.Pagination
{
    public static class CursorPaginationHelper
    {
        public static CursorPagedResult<TDto> Build<TEntity, TDto>(IReadOnlyList<TEntity> items,
            int pageSize,
            Func<TEntity, Cursor> cursorFactory,
            Func<TEntity, TDto> mapper)
        {
            var hasMore = items.Count > pageSize;

            var pageItems = hasMore
                ? items.Take(pageSize).ToList()
                : items;

            string? nextCursor = null;

            if (hasMore)
            {
                var last = pageItems[^1];
                var cursor = cursorFactory(last);
                nextCursor = CursorCodec.Encode(cursor);
            }

            return new CursorPagedResult<TDto>
            {
                Items = pageItems.Select(mapper).ToList(),
                NextCursor = nextCursor
            };
        }
    }
}
