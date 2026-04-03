using Edvantix.Persona.Infrastructure.Keycloak;

namespace Edvantix.Persona.Features.Admin.Profiles.Unblock;

public sealed record UnblockProfileCommand(Guid ProfileId) : ICommand;

public sealed class UnblockProfileCommandHandler(
    IProfileRepository repository,
    IKeycloakAdminService keycloakAdminService,
    ILogger<UnblockProfileCommandHandler> logger
) : ICommandHandler<UnblockProfileCommand>
{
    public async ValueTask<Unit> Handle(
        UnblockProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var spec = ProfileSpecification.MinimalForWrite(request.ProfileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, request.ProfileId);

        profile.Unblock();

        await keycloakAdminService.EnableUserAsync(profile.AccountId, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        logger.LogInformation(
            "Блокировка профиля {ProfileId} (аккаунт {AccountId}) снята администратором",
            request.ProfileId,
            profile.AccountId
        );

        return Unit.Value;
    }
}
