using Edvantix.Chassis.Utilities;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>
/// Команда загрузки нового аватара для текущего пользователя.
/// Возвращает обновлённый профиль с SAS-ссылкой на новый аватар.
/// </summary>
public sealed record UpdateAvatarCommand(IFormFile Avatar) : IRequest<ProfileViewModel>;

public sealed class UpdateAvatarCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateAvatarCommand, ProfileViewModel>
{
    public async ValueTask<ProfileViewModel> Handle(
        UpdateAvatarCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();
        var blobService = provider.GetRequiredService<IBlobService>();

        var spec = new ProfileByAccountIdSpec(accountId);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        var previousAvatarUrn = profile.AvatarUrl;

        // Загружаем новый аватар перед изменением сущности
        var newUrn = await blobService.UploadFileAsync(command.Avatar, ct);
        profile.UploadAvatar(newUrn);

        try
        {
            await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

            // Удаляем старый аватар только после успешного сохранения в БД
            if (previousAvatarUrn is not null)
                await blobService.DeleteFileAsync(previousAvatarUrn, ct);

            var mapper = provider.GetRequiredService<IMapper<Profile, ProfileViewModel>>();
            return mapper.Map(profile);
        }
        catch
        {
            // Если сохранение в БД не удалось — откатываем загруженный аватар
            await blobService.DeleteFileAsync(newUrn, ct);
            throw;
        }
    }
}
