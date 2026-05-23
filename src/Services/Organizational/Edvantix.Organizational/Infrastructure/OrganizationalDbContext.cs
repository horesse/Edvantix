using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Wolverine.EntityFrameworkCore;

namespace Edvantix.Organizational.Infrastructure;

public sealed class OrganizationalDbContext(DbContextOptions options)
    : DbContext(options),
        IUnitOfWork
{
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Permission> Permissions => Set<Permission>();

    // Organization aggregate
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Contact> Contacts => Set<Contact>();

    // OrganizationMember aggregate
    public DbSet<OrganizationMember> OrganizationMembers => Set<OrganizationMember>();
    public DbSet<OrganizationRole> OrganizationRoles => Set<OrganizationRole>();

    // Room aggregate
    public DbSet<Room> Rooms => Set<Room>();

    // Invitation aggregate
    public DbSet<Invitation> Invitations => Set<Invitation>();

    // StudentStatus directory
    public DbSet<StudentStatus> StudentStatuses => Set<StudentStatus>();

    // LeadSource directory
    public DbSet<LeadSource> LeadSources => Set<LeadSource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.MapWolverineEnvelopeStorage();
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrganizationalDbContext).Assembly);
    }

    public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {
        await SaveChangesAsync(cancellationToken);
        return true;
    }
}
