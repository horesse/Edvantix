using Edvantix.Contracts;

namespace Edvantix.Persona.Features.Admin.Profiles.Block;

public sealed record BlockProfileCommand(Guid ProfileId) : ICommand;

public sealed class BlockProfileCommandHandler(
    IProfileRepository repository,
    IPublishEndpoint publishEndpoint,
    ILogger<BlockProfileCommandHandler> logger
) : ICommandHandler<BlockProfileCommand>
{
    public async ValueTask<Unit> Handle(
        BlockProfileCommand request,
        CancellationToken cancellationToken
    )
    {
        var spec = ProfileSpecification.MinimalForWrite(request.ProfileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, request.ProfileId);

        profile.Block();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        await publishEndpoint.Publish(
            new DisableKeycloakUserIntegrationEvent(profile.AccountId),
            cancellationToken
        );

        logger.LogInformation(
            "Профиль {ProfileId} (аккаунт {AccountId}) заблокирован администратором",
            request.ProfileId,
            profile.AccountId
        );

        return Unit.Value;
    }
}
