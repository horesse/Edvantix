using Edvantix.Chassis.EF.Contexts;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.GroupMembershipHistoryAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

// Bank entity was intentionally removed from the domain. Delete Bank.cs if still present.

namespace Edvantix.Organizational.Infrastructure;

public sealed class OrganizationalDbContext(DbContextOptions options) : PostgresContext(options)
{
    public DbSet<Permission> Permissions => Set<Permission>();

    // Organization aggregate
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Contact> Contacts => Set<Contact>();

    // OrganizationMember aggregate
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<OrganizationMemberRole> OrganizationMemberRoles => Set<OrganizationMemberRole>();

    // Group aggregate
    public DbSet<Group> Groups => Set<Group>();
    public DbSet<GroupMember> GroupMembers => Set<GroupMember>();
    public DbSet<GroupRole> GroupRoles => Set<GroupRole>();

    // Group history aggregate
    public DbSet<GroupMembershipHistory> GroupMembershipHistories => Set<GroupMembershipHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationalDbContext).Assembly);
    }
}
