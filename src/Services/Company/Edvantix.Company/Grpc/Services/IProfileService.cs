namespace Edvantix.Company.Grpc.Services;

public interface IProfileService
{
    Task<long> GetProfileIdByAccountId(Guid accountId, CancellationToken cancellationToken);
}
