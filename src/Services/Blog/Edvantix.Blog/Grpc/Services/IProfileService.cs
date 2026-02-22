using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Grpc.Services;

public interface IProfileService
{
    Task<ulong> GetProfileIdByAccountId(Guid accountId, CancellationToken cancellationToken);

    Task<ProfileReply?> GetProfileById(ulong profileId, CancellationToken cancellationToken);
}
