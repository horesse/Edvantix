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
        await context
            .UserRoleAssignments.Where(a => a.ProfileId == profileId)
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
    public async Task<List<UserRoleAssignment>> GetByProfileAndSchoolAsync(
        Guid profileId,
        Guid schoolId,
        CancellationToken cancellationToken = default
    ) =>
        // IgnoreQueryFilters bypasses the tenant HasQueryFilter on UserRoleAssignment.
        // The schoolId is filtered explicitly so isolation guarantees are maintained —
        // this method is only called from gRPC paths that receive schoolId directly.
        await context
            .UserRoleAssignments.IgnoreQueryFilters()
            .Where(a => a.ProfileId == profileId && a.SchoolId == schoolId)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<List<UserRoleAssignment>> GetAllByRoleIdAsync(
        Guid roleId,
        CancellationToken cancellationToken = default
    ) =>
        // IgnoreQueryFilters bypasses the tenant HasQueryFilter so we get all users with
        // this role across all schools — required to enumerate affected cache entries when
        // a role's permission set changes.
        await context
            .UserRoleAssignments.IgnoreQueryFilters()
            .Where(a => a.RoleId == roleId)
            .ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public void Remove(UserRoleAssignment assignment) =>
        context.UserRoleAssignments.Remove(assignment);
}
