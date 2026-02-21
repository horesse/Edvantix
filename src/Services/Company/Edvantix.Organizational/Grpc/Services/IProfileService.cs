namespace Edvantix.Organizational.Grpc.Services;

public interface IProfileService
{
    Task<ulong> GetProfileIdByAccountId(Guid accountId, CancellationToken cancellationToken);
}
