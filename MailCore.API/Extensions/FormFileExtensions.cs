using MailCore.Application.Models;

namespace MailCore.API.Extensions;

public static class FormFileExtensions
{
    public static FileData ToFileData(this IFormFile file)
    {
        return new FileData(
            Path.GetFileName(file.FileName),
            string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType,
            file.Length,
            file.OpenReadStream);
    }

    public static IReadOnlyList<FileData> ToFileDataList(this IFormFileCollection? files)
    {
        if (files is null || files.Count == 0)
            return [];

        return files.Select(f => f.ToFileData()).ToList();
    }

    public static IReadOnlyList<FileData> ToFileDataList(this IReadOnlyList<IFormFile>? files)
    {
        if (files is null || files.Count == 0)
            return [];

        return files.Select(f => f.ToFileData()).ToList();
    }
}
