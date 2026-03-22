using Edvantix.Constants.Other;
using Edvantix.Persona.Infrastructure.Blob;
using Edvantix.Persona.Infrastructure.Keycloak;

namespace Edvantix.Persona.Features.Profiles.Registration;

public sealed class RegistrationCommand : ICommand<Guid>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateOnly BirthDate { get; init; }
    public Gender Gender { get; init; }
    public IFormFile? Avatar { get; init; }
}

public sealed class RegistrationCommandHandler(
    ClaimsPrincipal claims,
    IProfileRepository repository,
    IBlobService blobService,
    IKeycloakAdminService keycloakService
) : ICommandHandler<RegistrationCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        RegistrationCommand request,
        CancellationToken cancellationToken
    )
    {
        var accountId = claims.GetUserId();
        var login = claims.GetUserLogin();

        if (await repository.ExistsByAccountIdAsync(accountId, cancellationToken))
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
            avatarUrn = await blobService.UploadFileAsync(request.Avatar, cancellationToken);
            profile.UploadAvatar(avatarUrn);
        }

        try
        {
            await repository.AddAsync(profile, cancellationToken);
            await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
        }
        catch
        {
            if (avatarUrn is not null)
            {
                await blobService.DeleteFileAsync(avatarUrn, cancellationToken);
            }

            throw;
        }

        await keycloakService.SetProfileIdAsync(accountId, profile.Id, cancellationToken);

        return profile.Id;
    }
}
