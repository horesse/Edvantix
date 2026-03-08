using Edvantix.Chassis.Utilities;
using Edvantix.Constants.Other;
using Edvantix.Persona.Infrastructure.Blob;
using Edvantix.Persona.Infrastructure.Keycloak;

namespace Edvantix.Persona.Features.Profiles.Registration;

/// <summary>Команда первичной регистрации профиля пользователя. Возвращает внутренний ID профиля.</summary>
public sealed class RegistrationCommand : ICommand<Guid>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateOnly BirthDate { get; init; }
    public Gender Gender { get; init; }
    public IFormFile? Avatar { get; init; }
}

public sealed class RegistrationCommandHandler(IServiceProvider provider)
    : ICommandHandler<RegistrationCommand, Guid>
{
    public async ValueTask<Guid> Handle(RegistrationCommand request, CancellationToken ct)
    {
        var accountId = provider.GetUserId();
        var login = provider.GetUserLogin();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        if (await profileRepo.ExistsByAccountIdAsync(accountId, ct))
            throw new InvalidOperationException("Профиль для данного аккаунта уже существует.");

        var profile = new Profile(
            accountId,
            login,
            request.Gender,
            request.BirthDate,
            request.FirstName,
            request.LastName,
            request.MiddleName
        );

        string? avatarUrn = null;

        if (request.Avatar is not null)
        {
            var blobService = provider.GetRequiredService<IBlobService>();
            avatarUrn = await blobService.UploadFileAsync(request.Avatar, ct);
            profile.UploadAvatar(avatarUrn);
        }

        try
        {
            await profileRepo.AddAsync(profile, ct);
            await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);
        }
        catch
        {
            // Если сохранение в БД не удалось — удаляем загруженный аватар из хранилища
            if (avatarUrn is not null)
            {
                var blobService = provider.GetRequiredService<IBlobService>();
                await blobService.DeleteFileAsync(avatarUrn, ct);
            }

            throw;
        }

        // Сохраняем profileId в Keycloak как пользовательский атрибут.
        // Это позволяет связать Keycloak-аккаунт с профилем в Persona-сервисе
        // без дополнительного запроса к БД при каждом обращении.
        var keycloakAdmin = provider.GetRequiredService<IKeycloakAdminService>();
        await keycloakAdmin.SetProfileIdAsync(accountId, profile.Id, ct);

        return profile.Id;
    }
}
