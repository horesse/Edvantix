namespace Edvantix.Organizational.Grpc.Services;

public interface IProfileService
{
    Task<Guid> GetProfileIdByAccountId(Guid accountId, CancellationToken cancellationToken);
}
