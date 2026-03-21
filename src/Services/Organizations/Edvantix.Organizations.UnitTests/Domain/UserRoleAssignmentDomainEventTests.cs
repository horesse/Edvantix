using Edvantix.Organizations.Infrastructure.EventServices.Events;

namespace Edvantix.Organizations.UnitTests.Domain;

/// <summary>Tests that domain events are registered on UserRoleAssignment mutations.</summary>
public sealed class UserRoleAssignmentDomainEventTests
{
    [Test]
    public void GivenNewAssignment_WhenConstructed_ThenUserRoleAssignedEventRegistered()
    {
        var profileId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        var roleId = Guid.CreateVersion7();

        var assignment = new UserRoleAssignment(profileId, schoolId, roleId);

        var domainEvent = assignment.DomainEvents
            .OfType<UserRoleAssignedEvent>()
            .SingleOrDefault();
        domainEvent.ShouldNotBeNull();
        domainEvent.ProfileId.ShouldBe(profileId);
        domainEvent.SchoolId.ShouldBe(schoolId);
        domainEvent.RoleId.ShouldBe(roleId);
    }

    [Test]
    public void GivenExistingAssignment_WhenRevoked_ThenUserRoleRevokedEventRegistered()
    {
        var profileId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        var roleId = Guid.CreateVersion7();
        var assignment = new UserRoleAssignment(profileId, schoolId, roleId);
        assignment.ClearDomainEvents();

        assignment.Revoke();

        var domainEvent = assignment.DomainEvents
            .OfType<UserRoleRevokedEvent>()
            .SingleOrDefault();
        domainEvent.ShouldNotBeNull();
        domainEvent.ProfileId.ShouldBe(profileId);
        domainEvent.SchoolId.ShouldBe(schoolId);
        domainEvent.RoleId.ShouldBe(roleId);
    }

    [Test]
    public void GivenRole_WhenSetPermissions_ThenRolePermissionsChangedEventRegistered()
    {
        var schoolId = Guid.CreateVersion7();
        var role = new Role("Teacher", schoolId);
        role.Id = Guid.CreateVersion7();
        var permissionIds = new[] { Guid.CreateVersion7(), Guid.CreateVersion7() };

        role.SetPermissions(permissionIds);

        var domainEvent = role.DomainEvents
            .OfType<RolePermissionsChangedEvent>()
            .SingleOrDefault();
        domainEvent.ShouldNotBeNull();
        domainEvent.RoleId.ShouldBe(role.Id);
        domainEvent.SchoolId.ShouldBe(schoolId);
    }
}
