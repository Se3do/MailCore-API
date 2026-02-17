namespace MailService.Application.Common.Pagination
{
    public sealed class CursorPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public string? NextCursor { get; init; }
        public bool HasMore => NextCursor is not null;
    }
}
