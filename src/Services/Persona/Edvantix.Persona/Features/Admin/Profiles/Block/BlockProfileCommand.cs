using Edvantix.Contracts;
using Wolverine;

namespace Edvantix.Persona.Features.Admin.Profiles.Block;

public sealed record BlockProfileCommand(Guid ProfileId) : ICommand;

public sealed class BlockProfileCommandHandler(
    IProfileRepository repository,
    IMessageBus publishEndpoint,
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

        await publishEndpoint.PublishAsync(
            new DisableKeycloakUserIntegrationEvent(profile.AccountId)
        );

        logger.LogInformation(
            "Профиль {ProfileId} (аккаунт {AccountId}) заблокирован администратором",
            request.ProfileId,
            profile.AccountId
        );

        return Unit.Value;
    }
}
