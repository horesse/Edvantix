using Edvantix.Contracts;
using Edvantix.Organizations.Infrastructure.EventServices;
using Edvantix.Organizations.Infrastructure.EventServices.Events;

namespace Edvantix.Organizations.UnitTests.Infrastructure;

/// <summary>Tests that EventMapper correctly maps domain events to integration events.</summary>
public sealed class EventMapperTests
{
    private readonly EventMapper _mapper = new();

    [Test]
    public void GivenUserRoleAssignedEvent_WhenMapped_ThenReturnsUserPermissionsInvalidatedIntegrationEvent()
    {
        var profileId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        var roleId = Guid.CreateVersion7();
        var domainEvent = new UserRoleAssignedEvent(profileId, schoolId, roleId);

        var integrationEvent = _mapper.MapToIntegrationEvent(domainEvent);

        var result = integrationEvent.ShouldBeOfType<UserPermissionsInvalidatedIntegrationEvent>();
        result.UserId.ShouldBe(profileId);
        result.SchoolId.ShouldBe(schoolId);
    }

    [Test]
    public void GivenUserRoleRevokedEvent_WhenMapped_ThenReturnsUserPermissionsInvalidatedIntegrationEvent()
    {
        var profileId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        var roleId = Guid.CreateVersion7();
        var domainEvent = new UserRoleRevokedEvent(profileId, schoolId, roleId);

        var integrationEvent = _mapper.MapToIntegrationEvent(domainEvent);

        var result = integrationEvent.ShouldBeOfType<UserPermissionsInvalidatedIntegrationEvent>();
        result.UserId.ShouldBe(profileId);
        result.SchoolId.ShouldBe(schoolId);
    }

    [Test]
    public void GivenRolePermissionsChangedEvent_WhenMapped_ThenReturnsUserPermissionsInvalidatedIntegrationEvent()
    {
        var roleId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        var domainEvent = new RolePermissionsChangedEvent(roleId, schoolId);

        var integrationEvent = _mapper.MapToIntegrationEvent(domainEvent);

        var result = integrationEvent.ShouldBeOfType<UserPermissionsInvalidatedIntegrationEvent>();
        // UserId is null for role-level invalidation (affects all users with this role in the school)
        result.UserId.ShouldBeNull();
        result.SchoolId.ShouldBe(schoolId);
    }
}
