using Shouldly;

namespace Edvantix.Organizations.UnitTests.Domain;

/// <summary>
/// Unit tests for the <see cref="Group"/> domain aggregate.
/// Covers constructor validation, Update, and Delete behaviors.
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
    public void GivenNullOrWhitespaceName_WhenCreatingGroup_ThenThrowsArgumentException(string? name)
    {
        Should.Throw<ArgumentException>(() => new Group(name!, ValidSchoolId, ValidMaxCapacity, ValidColor));
    }

    [Test]
    public void GivenDefaultSchoolId_WhenCreatingGroup_ThenThrowsArgumentException()
    {
        Should.Throw<ArgumentException>(() => new Group(ValidName, Guid.Empty, ValidMaxCapacity, ValidColor));
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
    public void GivenNullOrWhitespaceName_WhenUpdatingGroup_ThenThrowsArgumentException(string? name)
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
}
