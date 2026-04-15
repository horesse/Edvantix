namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class GroupAggregateTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly DateOnly ValidStartDate = new(2024, 9, 1);
    private static readonly DateOnly ValidEndDate = new(2025, 6, 30);

    private static Group CreateValidGroup() =>
        new(
            ValidOrgId,
            "Группа А-1",
            "Группа первого курса факультета А",
            ValidStartDate,
            ValidEndDate
        );

    [Test]
    public void GivenValidData_WhenCreatingGroup_ThenShouldInitializePropertiesCorrectly()
    {
        var group = new Group(
            ValidOrgId,
            "Группа А-1",
            "Группа первого курса факультета А",
            ValidStartDate,
            ValidEndDate
        );

        group.OrganizationId.ShouldBe(ValidOrgId);
        group.Name.ShouldBe("Группа А-1");
        group.Description.ShouldBe("Группа первого курса факультета А");
        group.StartDate.ShouldBe(ValidStartDate);
        group.EndDate.ShouldBe(ValidEndDate);
        group.Status.ShouldBe(OrganizationStatus.Active);
        group.IsDeleted.ShouldBeFalse();
        group.Members.ShouldBeEmpty();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(Guid.Empty, "Группа А-1", "Описание", ValidStartDate, ValidEndDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreatingGroup_ThenShouldThrowArgumentException(
        string? name
    )
    {
        var act = () => new Group(ValidOrgId, name!, "Описание", ValidStartDate, ValidEndDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceDescription_WhenCreatingGroup_ThenShouldThrowArgumentException(
        string? description
    )
    {
        var act = () =>
            new Group(ValidOrgId, "Группа А-1", description!, ValidStartDate, ValidEndDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEndDateBeforeStartDate_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var endDateBeforeStart = ValidStartDate.AddDays(-1);

        var act = () =>
            new Group(ValidOrgId, "Группа А-1", "Описание", ValidStartDate, endDateBeforeStart);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEndDateEqualToStartDate_WhenCreatingGroup_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new Group(ValidOrgId, "Группа А-1", "Описание", ValidStartDate, ValidStartDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenActiveGroup_WhenUpdating_ThenShouldUpdateProperties()
    {
        var group = CreateValidGroup();
        var newEndDate = ValidEndDate.AddMonths(3);

        group.Update("Группа Б-2", "Обновлённое описание", newEndDate);

        group.Name.ShouldBe("Группа Б-2");
        group.Description.ShouldBe("Обновлённое описание");
        group.EndDate.ShouldBe(newEndDate);
    }

    [Test]
    public void GivenArchivedGroup_WhenUpdating_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateValidGroup();
        group.Archive();

        var act = () => group.Update("Группа Б-2", "Описание", ValidEndDate.AddMonths(1));

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenEndDateBeforeStartDate_WhenUpdating_ThenShouldThrowArgumentException()
    {
        var group = CreateValidGroup();
        var endDateBeforeStart = ValidStartDate.AddDays(-1);

        var act = () => group.Update("Группа А-1", "Описание", endDateBeforeStart);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenActiveGroup_WhenAddingMember_ThenShouldAddToMembers()
    {
        var group = CreateValidGroup();
        var member = new GroupMember(
            ValidOrgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            ValidStartDate
        );

        group.AddMember(member);

        group.Members.ShouldHaveSingleItem();
        group.Members[0].ShouldBe(member);
    }

    [Test]
    public void GivenArchivedGroup_WhenAddingMember_ThenShouldThrowInvalidOperationException()
    {
        var group = CreateValidGroup();
        group.Archive();
        var member = new GroupMember(
            ValidOrgId,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            ValidStartDate
        );

        var act = () => group.AddMember(member);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenNullMember_WhenAddingMember_ThenShouldThrowArgumentNullException()
    {
        var group = CreateValidGroup();

        var act = () => group.AddMember(null!);

        act.ShouldThrow<ArgumentNullException>();
    }

    [Test]
    public void GivenActiveGroup_WhenArchiving_ThenStatusShouldBeArchived()
    {
        var group = CreateValidGroup();

        group.Archive();

        group.Status.ShouldBe(OrganizationStatus.Archived);
        group.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenActiveGroup_WhenDeleting_ThenShouldMarkAsDeletedAndSetDeletedStatus()
    {
        var group = CreateValidGroup();

        group.Delete();

        group.IsDeleted.ShouldBeTrue();
        group.Status.ShouldBe(OrganizationStatus.Deleted);
    }

    [Test]
    public void GivenNameWithLeadingSpaces_WhenCreatingGroup_ThenNameShouldBeTrimmed()
    {
        var group = new Group(
            ValidOrgId,
            "  Группа А-1  ",
            "  Описание  ",
            ValidStartDate,
            ValidEndDate
        );

        group.Name.ShouldBe("Группа А-1");
        group.Description.ShouldBe("Описание");
    }
}
