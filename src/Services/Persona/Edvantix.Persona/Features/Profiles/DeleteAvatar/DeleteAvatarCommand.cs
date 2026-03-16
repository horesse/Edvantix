using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.DeleteAvatar;

/// <summary>DELETE /v1/profile/avatar — удалить аватар профиля.</summary>
public sealed class DeleteAvatarCommand : ICommand<ProfileDetailsModel>
{
    /// <summary>URN текущего аватара. Устанавливается обработчиком для удаления PostProcessor'ом.</summary>
    public string? AvatarUrn { get; set; }
}

public sealed class DeleteAvatarCommandHandler(IServiceProvider provider)
    : ICommandHandler<DeleteAvatarCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        DeleteAvatarCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        // Сохраняем URN для удаления из хранилища в PostProcessor'е.
        command.AvatarUrn = profile.AvatarUrl;

        profile.UploadAvatar(null);

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
