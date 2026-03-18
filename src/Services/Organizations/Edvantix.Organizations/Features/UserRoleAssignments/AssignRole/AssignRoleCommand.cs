using Edvantix.Organizations.Grpc.Services;

namespace Edvantix.Organizations.Features.UserRoleAssignments.AssignRole;

/// <summary>Command to assign a role to a user profile within the current tenant.</summary>
public sealed class AssignRoleCommand : ICommand<Guid>
{
    /// <summary>Gets the profile (user) identifier to assign the role to.</summary>
    public required Guid ProfileId { get; init; }

    /// <summary>Gets the role identifier to assign.</summary>
    public required Guid RoleId { get; init; }
}

/// <summary>
/// Handles role assignment with gRPC-based profile validation.
/// Validates the role exists in the current tenant, the profile exists in Persona,
/// and that the assignment does not already exist before creating it.
/// </summary>
public sealed class AssignRoleCommandHandler(
    IUserRoleAssignmentRepository assignmentRepository,
    IRoleRepository roleRepository,
    IPersonaProfileService personaProfileService,
    ITenantContext tenantContext
) : ICommandHandler<AssignRoleCommand, Guid>
{
    /// <inheritdoc/>
    public async ValueTask<Guid> Handle(
        AssignRoleCommand request,
        CancellationToken cancellationToken
    )
    {
        // 1. Validate role exists in the current tenant
        var role =
            await roleRepository.FindByIdAsync(request.RoleId, cancellationToken)
            ?? throw NotFoundException.For<Role>(request.RoleId);

        // 2. Validate profileId exists via gRPC to Persona (cross-service validation)
        var profileExists = await personaProfileService.ValidateProfileExistsAsync(
            request.ProfileId,
            cancellationToken
        );
        if (!profileExists)
        {
            throw new NotFoundException($"Profile with id {request.ProfileId} not found.");
        }

        // 3. Prevent duplicate assignment within the same tenant
        var existing = await assignmentRepository.FindAsync(
            request.ProfileId,
            request.RoleId,
            cancellationToken
        );
        if (existing is not null)
        {
            throw new InvalidOperationException(
                $"User {request.ProfileId} already has role '{role.Name}' in this school."
            );
        }

        // 4. Create and persist the assignment
        var assignment = new UserRoleAssignment(
            request.ProfileId,
            tenantContext.SchoolId,
            request.RoleId
        );
        await assignmentRepository.AddAsync(assignment, cancellationToken);
        await assignmentRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return assignment.Id;
    }
}
