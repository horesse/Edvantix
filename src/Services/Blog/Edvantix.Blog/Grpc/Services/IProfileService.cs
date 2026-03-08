using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Grpc.Services;

public interface IProfileService
{
    Task<ProfileReply?> GetProfileById(Guid profileId, CancellationToken cancellationToken);
}
