using Edvantix.Persona.Infrastructure.Keycloak;

namespace Edvantix.Persona.Features.Admin.Profiles.Block;

public sealed record BlockProfileCommand(Guid ProfileId) : ICommand;

public sealed class BlockProfileCommandHandler(
    IProfileRepository repository,
    IKeycloakAdminService keycloakAdminService,
    ILogger<BlockProfileCommandHandler> logger
) : ICommandHandler<BlockProfileCommand>
{
    public async ValueTask<Unit> Handle(BlockProfileCommand request, CancellationToken cancellationToken)
    {
        var spec = ProfileSpecification.MinimalForWrite(request.ProfileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, request.ProfileId);

        profile.Block();

        await keycloakAdminService.DisableUserAsync(profile.AccountId, cancellationToken);
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        logger.LogInformation(
            "Профиль {ProfileId} (аккаунт {AccountId}) заблокирован администратором",
            request.ProfileId,
            profile.AccountId
        );

        return Unit.Value;
    }
}
