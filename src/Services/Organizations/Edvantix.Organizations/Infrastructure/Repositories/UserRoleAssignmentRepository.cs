using Edvantix.Organizations.Domain.AggregatesModel.UserRoleAssignmentAggregate;

namespace Edvantix.Organizations.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IUserRoleAssignmentRepository"/>.
/// The DbContext applies the tenant query filter automatically — all queries here
/// return only assignments for the current school.
/// </summary>
public sealed class UserRoleAssignmentRepository(OrganizationsDbContext context)
    : IUserRoleAssignmentRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<UserRoleAssignment?> FindAsync(
        Guid profileId,
        Guid roleId,
        CancellationToken cancellationToken = default
    ) =>
        await context.UserRoleAssignments.FirstOrDefaultAsync(
            a => a.ProfileId == profileId && a.RoleId == roleId,
            cancellationToken
        );

    /// <inheritdoc/>
    public async Task<List<UserRoleAssignment>> GetByProfileIdAsync(
        Guid profileId,
        CancellationToken cancellationToken = default
    ) =>
        await context.UserRoleAssignments
            .Where(a => a.ProfileId == profileId)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> ExistsByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    ) => await context.UserRoleAssignments.AnyAsync(a => a.RoleId == roleId, cancellationToken);

    /// <inheritdoc/>
    public async Task<UserRoleAssignment> AddAsync(
        UserRoleAssignment assignment,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await context.UserRoleAssignments.AddAsync(assignment, cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public void Remove(UserRoleAssignment assignment) =>
        context.UserRoleAssignments.Remove(assignment);
}
