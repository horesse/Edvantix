using Edvantix.ArchTests.Abstractions;
using Edvantix.Chassis.Security.Tenant;
using Edvantix.Organizations.Domain.Abstractions;
using Edvantix.Organizations.Domain.AggregatesModel.PermissionAggregate;
using Edvantix.Organizations.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Edvantix.ArchTests.Domain;

/// <summary>
/// Architecture tests that verify the tenant isolation convention for the Organizations service.
/// Uses EF Core model introspection (not ArchUnitNET IL scanning) to confirm that every
/// entity implementing <see cref="ITenanted"/> has a registered HasQueryFilter.
/// These tests enforce a contract: any new ITenanted entity added in future plans
/// MUST have a corresponding filter registered in OrganizationsDbContext.
/// </summary>
public sealed class TenantIsolationTests : BaseTest
{
    [Test]
    public void GivenTenantedEntities_WhenCheckingOrganizationsDbContextModel_ThenAllShouldHaveQueryFilter()
    {
        // Arrange: build OrganizationsDbContext with in-memory provider and a resolved stub tenant
        var tenantContext = new TenantContext();
        tenantContext.Resolve(Guid.NewGuid()); // any valid GUID — needed so the filter expression compiles

        var options = new DbContextOptionsBuilder<OrganizationsDbContext>()
            .UseInMemoryDatabase(databaseName: $"TenantIsolationTest_{Guid.NewGuid()}")
            .Options;

        using var dbContext = new OrganizationsDbContext(options, tenantContext);

        // Act: find all entity types in the model that implement ITenanted
        var tenantedEntityTypes = dbContext
            .Model.GetEntityTypes()
            .Where(et => et.ClrType.IsAssignableTo(typeof(ITenanted)))
            .ToList();

        // Assert: at least one ITenanted entity must exist (sanity check for future regressions)
        tenantedEntityTypes.Count.ShouldBeGreaterThan(
            0,
            "Expected at least one ITenanted entity in OrganizationsDbContext"
        );

        foreach (var entityType in tenantedEntityTypes)
        {
            var queryFilters = entityType.GetDeclaredQueryFilters().ToList();

            queryFilters.ShouldNotBeEmpty(
                $"Entity {entityType.ClrType.Name} implements ITenanted but has no HasQueryFilter. "
                    + "Add HasQueryFilter in OrganizationsDbContext.OnModelCreating."
            );
        }
    }

    [Test]
    public void GivenPermissionEntity_WhenCheckingModel_ThenShouldNotHaveQueryFilter()
    {
        // Permission is a global catalogue — it must NOT be tenant-scoped
        var tenantContext = new TenantContext();
        tenantContext.Resolve(Guid.NewGuid());

        var options = new DbContextOptionsBuilder<OrganizationsDbContext>()
            .UseInMemoryDatabase(databaseName: $"PermissionFilterTest_{Guid.NewGuid()}")
            .Options;

        using var dbContext = new OrganizationsDbContext(options, tenantContext);

        var permissionEntityType = dbContext.Model.FindEntityType(typeof(Permission));

        permissionEntityType.ShouldNotBeNull();
        permissionEntityType
            .GetDeclaredQueryFilters()
            .ShouldBeEmpty(
                "Permission is a global catalogue and must NOT have a tenant query filter."
            );
    }
}
