namespace SchoolLMS.Application.DTOs.Common;

public class FileUploadInput : IAsyncDisposable
{
    public required string FileName { get; init; }
    public string? ContentType { get; init; }
    public long Length { get; init; }
    public required Stream Content { get; init; }

    public ValueTask DisposeAsync()
    {
        return Content.DisposeAsync();
    }
}
