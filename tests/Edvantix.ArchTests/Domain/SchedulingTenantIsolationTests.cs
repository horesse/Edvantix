using Edvantix.ArchTests.Abstractions;
using Edvantix.Chassis.Security.Tenant;
using Edvantix.Scheduling.Infrastructure;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace Edvantix.ArchTests.Domain;

/// <summary>
/// Architecture tests that verify the tenant isolation convention for the Scheduling service.
/// Uses EF Core model introspection (not ArchUnitNET IL scanning) to confirm that every
/// entity implementing <see cref="ITenanted"/> has a registered HasQueryFilter.
/// These tests enforce a contract: any new ITenanted entity added to Scheduling in future plans
/// MUST have a corresponding filter registered in SchedulingDbContext.
/// </summary>
public sealed class SchedulingTenantIsolationTests : BaseTest
{
    [Test]
    public void GivenTenantedEntities_WhenCheckingSchedulingDbContextModel_ThenAllShouldHaveQueryFilter()
    {
        // Arrange: build SchedulingDbContext with in-memory provider and a resolved stub tenant
        var tenantContext = new TenantContext();
        tenantContext.Resolve(Guid.NewGuid()); // any valid GUID — needed so the filter expression compiles

        var options = new DbContextOptionsBuilder<SchedulingDbContext>()
            .UseInMemoryDatabase(databaseName: $"SchedulingTenantIsolationTest_{Guid.NewGuid()}")
            .Options;

        using var dbContext = new SchedulingDbContext(options, tenantContext);

        // Act: find all entity types in the model that implement ITenanted (Edvantix.SharedKernel.SeedWork.ITenanted)
        var tenantedEntityTypes = dbContext
            .Model.GetEntityTypes()
            .Where(et => et.ClrType.IsAssignableTo(typeof(ITenanted)))
            .ToList();

        // Assert: at least 1 ITenanted entity exists — LessonSlot (Group lives in Organizations, not here)
        tenantedEntityTypes.Count.ShouldBeGreaterThan(
            0,
            "Expected at least one ITenanted entity in SchedulingDbContext (LessonSlot). "
                + "Group and GroupMembership live in the Organizations service (D-15)."
        );

        foreach (var entityType in tenantedEntityTypes)
        {
            var queryFilters = entityType.GetDeclaredQueryFilters().ToList();

            queryFilters.ShouldNotBeEmpty(
                $"Entity {entityType.ClrType.Name} implements ITenanted but has no HasQueryFilter. "
                    + "Add HasQueryFilter in SchedulingDbContext.OnModelCreating."
            );
        }
    }
}
