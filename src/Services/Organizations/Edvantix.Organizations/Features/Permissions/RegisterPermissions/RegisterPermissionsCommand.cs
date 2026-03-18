namespace Edvantix.Organizations.Features.Permissions.RegisterPermissions;

/// <summary>
/// Command to register (upsert) a set of permission strings into the global catalogue.
/// This endpoint is intended for other services to call during their startup to register
/// their own permission strings. The operation is idempotent — existing names are skipped.
/// </summary>
public sealed class RegisterPermissionsCommand : ICommand
{
    /// <summary>Gets the list of permission strings to register.</summary>
    public required List<string> PermissionNames { get; init; }
}

/// <summary>Handles permission registration by upserting the provided names into the catalogue.</summary>
public sealed class RegisterPermissionsCommandHandler(IPermissionRepository permissionRepository)
    : ICommandHandler<RegisterPermissionsCommand>
{
    /// <inheritdoc/>
    public async ValueTask<Unit> Handle(
        RegisterPermissionsCommand request,
        CancellationToken cancellationToken
    )
    {
        await permissionRepository.UpsertAsync(request.PermissionNames, cancellationToken);
        await permissionRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return Unit.Value;
    }
}
