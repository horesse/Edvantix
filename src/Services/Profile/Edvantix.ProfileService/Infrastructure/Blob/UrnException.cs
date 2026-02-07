namespace Edvantix.ProfileService.Infrastructure.Blob;

public sealed class UrnException(string message, string paramName = "urn")
    : ArgumentException(message, paramName);
