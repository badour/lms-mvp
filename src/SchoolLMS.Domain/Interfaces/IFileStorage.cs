namespace SchoolLMS.Domain.Interfaces;

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, string originalFileName, string folder, CancellationToken cancellationToken = default);
    Task<Stream> OpenReadAsync(string relativePath, CancellationToken cancellationToken = default);
    Task DeleteAsync(string relativePath, CancellationToken cancellationToken = default);
}
