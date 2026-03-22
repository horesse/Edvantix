namespace Edvantix.Persona.Features.Profiles.DeleteAvatar;

public sealed class DeleteAvatarCommand : ICommand<Guid>;

public sealed class DeleteAvatarCommandHandler(
    IProfileRepository repository,
    ClaimsPrincipal claims
) : ICommandHandler<DeleteAvatarCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        DeleteAvatarCommand command,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.GetProfileIdOrError();

        var spec = ProfileSpecification.ForWrite(profileId);
        var profile =
            await repository.FindAsync(spec, cancellationToken)
            ?? throw new NotFoundException("Профиль не найден.");

        profile.DeleteAvatar();

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return profileId;
    }
}
