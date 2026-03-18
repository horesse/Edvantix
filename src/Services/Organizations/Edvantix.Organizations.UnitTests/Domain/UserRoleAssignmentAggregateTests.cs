namespace Edvantix.Organizations.UnitTests.Domain;

public sealed class UserRoleAssignmentAggregateTests
{
    [Test]
    public void GivenValidIds_WhenCreatingAssignment_ThenAllFieldsSet()
    {
        var profileId = Guid.CreateVersion7();
        var schoolId = Guid.CreateVersion7();
        var roleId = Guid.CreateVersion7();

        var assignment = new UserRoleAssignment(profileId, schoolId, roleId);

        assignment.ProfileId.ShouldBe(profileId);
        assignment.SchoolId.ShouldBe(schoolId);
        assignment.RoleId.ShouldBe(roleId);
    }

    [Test]
    public void GivenEmptyProfileId_WhenCreatingAssignment_ThenThrowsArgumentException()
    {
        var act = () =>
            new UserRoleAssignment(Guid.Empty, Guid.CreateVersion7(), Guid.CreateVersion7());

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptySchoolId_WhenCreatingAssignment_ThenThrowsArgumentException()
    {
        var act = () =>
            new UserRoleAssignment(Guid.CreateVersion7(), Guid.Empty, Guid.CreateVersion7());

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyRoleId_WhenCreatingAssignment_ThenThrowsArgumentException()
    {
        var act = () =>
            new UserRoleAssignment(Guid.CreateVersion7(), Guid.CreateVersion7(), Guid.Empty);

        act.ShouldThrow<ArgumentException>();
    }
}
