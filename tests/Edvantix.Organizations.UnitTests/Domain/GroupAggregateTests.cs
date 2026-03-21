using Shouldly;

namespace Edvantix.Organizations.UnitTests.Domain;

/// <summary>
/// Unit tests for the <see cref="Group"/> domain aggregate.
/// Covers constructor validation, Update, Delete, and GroupMembership behaviors.
/// </summary>
public sealed class GroupAggregateTests
{
    private static readonly Guid ValidSchoolId = Guid.NewGuid();
    private const string ValidName = "Mathematics A";
    private const int ValidMaxCapacity = 25;
    private const string ValidColor = "#FF5733";

    [Test]
    public void GivenValidArguments_WhenCreatingGroup_ThenPropertiesAreSet()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);

        group.Name.ShouldBe(ValidName);
        group.SchoolId.ShouldBe(ValidSchoolId);
        group.MaxCapacity.ShouldBe(ValidMaxCapacity);
        group.Color.ShouldBe(ValidColor);
        group.IsDeleted.ShouldBeFalse();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhitespaceName_WhenCreatingGroup_ThenThrowsArgumentException(
        string? name
    )
    {
        Should.Throw<ArgumentException>(() =>
            new Group(name!, ValidSchoolId, ValidMaxCapacity, ValidColor)
        );
    }

    [Test]
    public void GivenDefaultSchoolId_WhenCreatingGroup_ThenThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() =>
            new Group(ValidName, Guid.Empty, ValidMaxCapacity, ValidColor)
        );
    }

    [Test]
    public void GivenValidArguments_WhenUpdatingGroup_ThenPropertiesAreUpdated()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);
        const string newName = "Physics B";
        const int newMaxCapacity = 30;
        const string newColor = "#00FF00";

        group.Update(newName, newMaxCapacity, newColor);

        group.Name.ShouldBe(newName);
        group.MaxCapacity.ShouldBe(newMaxCapacity);
        group.Color.ShouldBe(newColor);
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhitespaceName_WhenUpdatingGroup_ThenThrowsArgumentException(
        string? name
    )
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);

        Should.Throw<ArgumentException>(() => group.Update(name!, ValidMaxCapacity, ValidColor));
    }

    [Test]
    public void GivenExistingGroup_WhenDeleting_ThenIsDeletedIsTrue()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);

        group.Delete();

        group.IsDeleted.ShouldBeTrue();
    }

    // GroupMembership tests

    [Test]
    public void GivenNewProfileId_WhenAddingMember_ThenMemberIsAdded()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);
        group.Id = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();

        group.AddMember(profileId, DateTimeOffset.UtcNow);

        group.Members.Count.ShouldBe(1);
        group.Members.First().ProfileId.ShouldBe(profileId);
    }

    [Test]
    public void GivenExistingProfileId_WhenAddingMemberAgain_ThenMemberCountRemainsOne()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);
        group.Id = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var addedAt = DateTimeOffset.UtcNow;

        group.AddMember(profileId, addedAt);
        group.AddMember(profileId, addedAt.AddMinutes(1)); // duplicate — should be a no-op per SCH-06

        group.Members.Count.ShouldBe(1);
    }

    [Test]
    public void GivenExistingMember_WhenRemovingMember_ThenMemberIsRemoved()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);
        group.Id = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        group.AddMember(profileId, DateTimeOffset.UtcNow);

        group.RemoveMember(profileId);

        group.Members.Count.ShouldBe(0);
    }

    [Test]
    public void GivenNonExistingMember_WhenRemovingMember_ThenMemberCountRemainsZero()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);
        group.Id = Guid.CreateVersion7();

        group.RemoveMember(Guid.CreateVersion7()); // no-op — should not throw

        group.Members.Count.ShouldBe(0);
    }

    [Test]
    public void GivenNewMember_WhenAdded_ThenMembershipHasCorrectSchoolAndGroupIds()
    {
        var group = new Group(ValidName, ValidSchoolId, ValidMaxCapacity, ValidColor);
        group.Id = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var addedAt = DateTimeOffset.UtcNow;

        group.AddMember(profileId, addedAt);

        var membership = group.Members.First();
        membership.GroupId.ShouldBe(group.Id);
        membership.SchoolId.ShouldBe(ValidSchoolId);
        membership.AddedAt.ShouldBe(addedAt);
    }
}
