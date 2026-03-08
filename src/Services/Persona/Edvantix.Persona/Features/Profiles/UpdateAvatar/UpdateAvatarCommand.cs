using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>PATCH /v1/profile/avatar — загрузить или заменить аватар профиля.</summary>
public sealed class UpdateAvatarCommand : IRequest<ProfileDetailsModel>
{
    /// <summary>Новый аватар пользователя (JPEG/PNG до 1 МБ).</summary>
    public required IFormFile Avatar { get; init; }

    /// <summary>URN загруженного аватара. Устанавливается PreProcessor'ом.</summary>
    public string? AvatarUrn { get; set; }

    /// <summary>URN предыдущего аватара. Устанавливается обработчиком для удаления PostProcessor'ом.</summary>
    public string? OldAvatarUrn { get; set; }
}

public sealed class UpdateAvatarCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateAvatarCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdateAvatarCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        if (command.AvatarUrn is not null)
        {
            command.OldAvatarUrn = profile.AvatarUrl;
            profile.UploadAvatar(command.AvatarUrn);
        }

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
