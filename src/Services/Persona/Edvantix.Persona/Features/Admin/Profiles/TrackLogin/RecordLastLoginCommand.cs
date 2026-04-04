namespace Edvantix.Persona.Features.Admin.Profiles.TrackLogin;

public sealed record RecordLastLoginCommand : ICommand;

public sealed class RecordLastLoginCommandHandler(
    IProfileRepository repository,
    ClaimsPrincipal claims
) : ICommandHandler<RecordLastLoginCommand>
{
    public async ValueTask<Unit> Handle(
        RecordLastLoginCommand request,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.TryGetProfileId();

        if (profileId is null || profileId == Guid.Empty)
        {
            return Unit.Value;
        }

        var spec = ProfileSpecification.MinimalForWrite(profileId.Value);
        var profile = await repository.FindAsync(spec, cancellationToken);

        if (profile is null)
        {
            return Unit.Value;
        }

        profile.RecordLastLogin();
        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
