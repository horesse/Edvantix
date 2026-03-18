using Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizations.Domain.AggregatesModel.RoleAggregate;
using Edvantix.Organizations.Domain.AggregatesModel.UserRoleAssignmentAggregate;

namespace Edvantix.Organizations.Infrastructure;

/// <summary>
/// EF Core DbContext for the Organizations service.
/// Applies tenant isolation query filters in <see cref="OnModelCreating"/> rather than
/// in individual entity configurations, because filter expressions require access to the
/// injected <see cref="ITenantContext"/> which is not available inside
/// <c>ApplyConfigurationsFromAssembly</c>.
/// </summary>
public sealed class OrganizationsDbContext(
    DbContextOptions<OrganizationsDbContext> options,
    ITenantContext tenantContext
) : DbContext(options), IUnitOfWork
{
    /// <summary>Gets the roles data set.</summary>
    public DbSet<Role> Roles => Set<Role>();

    /// <summary>Gets the permissions data set (global catalogue).</summary>
    public DbSet<Permission> Permissions => Set<Permission>();

    /// <summary>Gets the user-role assignments data set.</summary>
    public DbSet<UserRoleAssignment> UserRoleAssignments => Set<UserRoleAssignment>();

    /// <summary>Gets the role-permission links data set.</summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationsDbContext).Assembly);

        // Tenant isolation query filters.
        // CRITICAL: EF Core supports only one HasQueryFilter per entity.
        // For Role, combine tenant + soft-delete into a single expression.
        modelBuilder
            .Entity<Role>()
            .HasQueryFilter(r => r.SchoolId == tenantContext.SchoolId && !r.IsDeleted);

        modelBuilder
            .Entity<UserRoleAssignment>()
            .HasQueryFilter(a => a.SchoolId == tenantContext.SchoolId);

        // Permission: NO query filter — global catalogue, not tenant-scoped.
    }

    /// <inheritdoc/>
    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
