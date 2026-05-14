namespace MailCore.Application.Common.Pagination
{
    /// <summary>Result of a paginated query with cursor-based navigation.</summary>
    public sealed class CursorPagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public string? NextCursor { get; init; }
        public bool HasMore => NextCursor is not null;
    }
}
