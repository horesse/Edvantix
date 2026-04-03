using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Admin.Profiles;

public sealed class AdminDomainToDtoMapper(IBlobService blobService) : Mapper<Profile, AdminProfileDto>
{
    public override AdminProfileDto Map(Profile source)
    {
        var avatarUrl = source.AvatarUrl is not null
            ? blobService.GetFileSasUrl(source.AvatarUrl)
            : null;

        return new AdminProfileDto(
            source.Id,
            source.AccountId,
            source.FullName.GetFullName(),
            source.Login,
            avatarUrl,
            source.IsBlocked,
            source.LastLoginAt
        );
    }
}
