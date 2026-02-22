using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.Mappers;

/// <summary>
/// Маппер Profile → ProfileViewModel (краткое представление).
/// SAS-ссылка на аватар генерируется при маппинге.
/// </summary>
public sealed class ProfileViewModelMapper(IBlobService blobService)
    : Mapper<Profile, ProfileViewModel>
{
    public override ProfileViewModel Map(Profile source)
    {
        var avatarUrl = source.AvatarUrl is not null
            ? blobService.GetFileSasUrl(source.AvatarUrl)
            : null;

        return new ProfileViewModel(
            source.Id,
            source.FullName.GetFullName(),
            source.Login,
            avatarUrl
        );
    }
}
