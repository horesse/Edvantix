namespace Edvantix.ProfileService.Infrastructure.Blob;

public interface IBlobService
{
    Task<string> UploadFileAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task DeleteFileAsync(string urn, CancellationToken cancellationToken = default);
    string GetFileSasUrl(string urn);
}
