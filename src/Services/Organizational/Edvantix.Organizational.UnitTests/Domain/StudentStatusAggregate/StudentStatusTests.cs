using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;

namespace Edvantix.Organizational.UnitTests.Domain.StudentStatusAggregate;

public sealed class StudentStatusTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();
    private static readonly Guid UserId = Guid.CreateVersion7();

    [Test]
    public void GivenValidData_WhenCreating_ThenPropertiesAreInitialized()
    {
        var status = new StudentStatus(
            OrgId,
            "  Активный  ",
            "ACTIVE",
            StudentStatusTone.Active,
            isSystem: false,
            order: 5,
            createdBy: UserId
        );

        status.OrganizationId.ShouldBe(OrgId);
        status.Name.ShouldBe("Активный");
        status.Code.ShouldBe("ACTIVE");
        status.Tone.ShouldBe(StudentStatusTone.Active);
        status.IsSystem.ShouldBeFalse();
        status.Order.ShouldBe(5);
        status.IsArchived.ShouldBeFalse();
        status.CreatedBy.ShouldBe(UserId);
        status.Id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreating_ThenShouldThrow()
    {
        Action act = () =>
            new StudentStatus(Guid.Empty, "Активный", "ACT", StudentStatusTone.Active);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceName_WhenCreating_ThenShouldThrow(string? name)
    {
        Action act = () => new StudentStatus(OrgId, name!, "ACT", StudentStatusTone.Active);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenNameLongerThanMax_WhenCreating_ThenShouldThrow()
    {
        var name = new string('A', 121);

        Action act = () => new StudentStatus(OrgId, name, "ACT", StudentStatusTone.Active);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public void GivenNullOrWhiteSpaceCode_WhenCreating_ThenShouldThrow(string? code)
    {
        Action act = () => new StudentStatus(OrgId, "Активный", code!, StudentStatusTone.Active);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenCodeLongerThan20_WhenCreating_ThenShouldThrow()
    {
        var code = new string('X', StudentStatus.MaxCodeLength + 1);

        Action act = () => new StudentStatus(OrgId, "Активный", code, StudentStatusTone.Active);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenCodeAtMaxLength_WhenCreating_ThenShouldSucceed()
    {
        var code = new string('X', StudentStatus.MaxCodeLength);

        var status = new StudentStatus(OrgId, "Активный", code, StudentStatusTone.Active);

        status.Code.ShouldBe(code);
    }

    [Test]
    public void GivenNonSystemStatus_WhenArchiving_ThenShouldBecomeArchived()
    {
        var status = new StudentStatus(
            OrgId,
            "Пользовательский",
            "CUSTOM",
            StudentStatusTone.Neutral
        );

        status.Archive(UserId);

        status.IsArchived.ShouldBeTrue();
        status.LastModifiedBy.ShouldBe(UserId);
    }

    [Test]
    public void GivenSystemStatus_WhenArchiving_ThenShouldThrowInvalidOperation()
    {
        var status = new StudentStatus(
            OrgId,
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            isSystem: true
        );

        var act = () => status.Archive(UserId);

        act.ShouldThrow<InvalidOperationException>();
        status.IsArchived.ShouldBeFalse();
    }

    [Test]
    public void GivenArchivedNonSystemStatus_WhenArchivingAgain_ThenShouldBeNoop()
    {
        var status = new StudentStatus(
            OrgId,
            "Пользовательский",
            "CUSTOM",
            StudentStatusTone.Neutral
        );
        status.Archive(UserId);
        var firstModifiedAt = status.LastModifiedAt;

        status.Archive(Guid.CreateVersion7());

        status.IsArchived.ShouldBeTrue();
        status.LastModifiedAt.ShouldBe(firstModifiedAt);
    }

    [Test]
    public void GivenNonSystemArchivedStatus_WhenRestoring_ThenShouldBecomeActive()
    {
        var status = new StudentStatus(
            OrgId,
            "Пользовательский",
            "CUSTOM",
            StudentStatusTone.Neutral
        );
        status.Archive(UserId);

        var restoredBy = Guid.CreateVersion7();
        status.Restore(restoredBy);

        status.IsArchived.ShouldBeFalse();
        status.LastModifiedBy.ShouldBe(restoredBy);
    }

    [Test]
    public void GivenSystemStatus_WhenRestoring_ThenShouldThrowInvalidOperation()
    {
        var status = new StudentStatus(
            OrgId,
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            isSystem: true
        );

        var act = () => status.Restore(UserId);

        act.ShouldThrow<InvalidOperationException>();
    }

    [Test]
    public void GivenValidData_WhenUpdating_ThenPropertiesAreChanged()
    {
        var status = new StudentStatus(OrgId, "Активный", "ACTIVE", StudentStatusTone.Active);

        status.Update("Выпускник", "GRAD", StudentStatusTone.Neutral, 5, UserId);

        status.Name.ShouldBe("Выпускник");
        status.Code.ShouldBe("GRAD");
        status.Tone.ShouldBe(StudentStatusTone.Neutral);
        status.Order.ShouldBe(5);
        status.LastModifiedBy.ShouldBe(UserId);
    }

    [Test]
    public void GivenInvalidCode_WhenUpdating_ThenShouldThrow()
    {
        var status = new StudentStatus(OrgId, "Активный", "ACTIVE", StudentStatusTone.Active);
        var tooLongCode = new string('X', StudentStatus.MaxCodeLength + 1);

        var act = () => status.Update("Активный", tooLongCode, StudentStatusTone.Active, 0, UserId);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenDefaultFactory_WhenCreating_ThenShouldReturnFourSystemStatuses()
    {
        var statuses = DefaultStudentStatusesFactory.CreateFor(OrgId);

        statuses.Count.ShouldBe(4);
        statuses.ShouldAllBe(s => s.IsSystem);
        statuses.ShouldAllBe(s => s.OrganizationId == OrgId);
        statuses.Select(s => s.Code).Distinct().Count().ShouldBe(4);
    }

    [Test]
    public void GivenDefaultFactory_WhenCreating_ThenShouldContainExpectedStatuses()
    {
        var statuses = DefaultStudentStatusesFactory.CreateFor(OrgId);
        var codes = statuses.Select(s => s.Code).ToHashSet();

        codes.ShouldContain("ACTIVE");
        codes.ShouldContain("ON_LEAVE");
        codes.ShouldContain("GRADUATE");
        codes.ShouldContain("EXPELLED");
    }
}
