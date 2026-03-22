using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles;

public sealed class DomainToDtoMapper(IBlobService blobService) : Mapper<Profile, ProfileDto>
{
    public override ProfileDto Map(Profile source)
    {
        var avatarUrl = source.AvatarUrl is not null
            ? blobService.GetFileSasUrl(source.AvatarUrl)
            : null;

        return new ProfileDto(source.Id, source.FullName.GetFullName(), source.Login, avatarUrl);
    }
}
