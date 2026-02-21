using Edvantix.Chassis.Utilities;
using Edvantix.Constants.Other;
using Edvantix.Persona.Infrastructure.Blob;

namespace Edvantix.Persona.Features.Profiles.Registration;

/// <summary>Команда первичной регистрации профиля пользователя. Возвращает внутренний ID профиля.</summary>
public sealed class RegistrationCommand : IRequest<ulong>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateOnly BirthDate { get; init; }
    public Gender Gender { get; init; }
    public IFormFile? Avatar { get; init; }
}

public sealed class RegistrationCommandHandler(IServiceProvider provider)
    : IRequestHandler<RegistrationCommand, ulong>
{
    public async ValueTask<ulong> Handle(RegistrationCommand request, CancellationToken ct)
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
            return profile.Id;
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
    }
}
