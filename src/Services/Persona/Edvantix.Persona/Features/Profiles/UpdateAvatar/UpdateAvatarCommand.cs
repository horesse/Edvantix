using Edvantix.Chassis.Utilities;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>PATCH /v1/profile/avatar — загрузить или заменить аватар профиля.</summary>
public sealed class UpdateAvatarCommand : ICommand<ProfileDetailsModel>
{
    /// <summary>Новый аватар пользователя (JPEG/PNG до 1 МБ).</summary>
    public required IFormFile Avatar { get; init; }
}

public sealed class UpdateAvatarCommandHandler(IServiceProvider provider)
    : ICommandHandler<UpdateAvatarCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdateAvatarCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var blobService = provider.GetRequiredService<IBlobService>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        // Загружаем новый файл в хранилище до сохранения профиля.
        var newAvatarUrn = await blobService.UploadFileAsync(command.Avatar, ct);

        try
        {
            // UploadAvatar регистрирует AvatarDeletedDomainEvent для старого блоба, если он был.
            profile.UploadAvatar(newAvatarUrn);

            await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);
        }
        catch
        {
            // Если сохранение не удалось — удаляем только что загруженный блоб,
            // чтобы не оставлять осиротевших файлов в хранилище.
            await blobService.DeleteFileAsync(newAvatarUrn, ct);
            throw;
        }

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
