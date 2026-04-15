namespace Edvantix.Organizational.UnitTests.Domain;

public sealed class OrganizationMemberAggregateTests
{
    private static readonly Guid ValidOrgId = Guid.CreateVersion7();
    private static readonly Guid ValidProfileId = Guid.CreateVersion7();
    private static readonly Guid ValidRoleId = Guid.CreateVersion7();
    private static readonly DateOnly ValidStartDate = new(2024, 1, 1);

    private static OrganizationMember CreateValidMember(DateOnly? endDate = null) =>
        new(ValidOrgId, ValidProfileId, ValidRoleId, ValidStartDate, endDate);

    [Test]
    public void GivenValidData_WhenCreatingOrganizationMember_ThenShouldInitializePropertiesCorrectly()
    {
        var endDate = new DateOnly(2024, 12, 31);

        var member = new OrganizationMember(
            ValidOrgId,
            ValidProfileId,
            ValidRoleId,
            ValidStartDate,
            endDate
        );

        member.OrganizationId.ShouldBe(ValidOrgId);
        member.ProfileId.ShouldBe(ValidProfileId);
        member.OrganizationMemberRoleId.ShouldBe(ValidRoleId);
        member.StartDate.ShouldBe(ValidStartDate);
        member.EndDate.ShouldBe(endDate);
        member.Status.ShouldBe(OrganizationStatus.Active);
        member.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public void GivenNoEndDate_WhenCreatingOrganizationMember_ThenEndDateShouldBeNull()
    {
        var member = CreateValidMember();

        member.EndDate.ShouldBeNull();
    }

    [Test]
    public void GivenEmptyOrganizationId_WhenCreatingOrganizationMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new OrganizationMember(Guid.Empty, ValidProfileId, ValidRoleId, ValidStartDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyProfileId_WhenCreatingOrganizationMember_ThenShouldThrowArgumentException()
    {
        var act = () => new OrganizationMember(ValidOrgId, Guid.Empty, ValidRoleId, ValidStartDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEmptyRoleId_WhenCreatingOrganizationMember_ThenShouldThrowArgumentException()
    {
        var act = () =>
            new OrganizationMember(ValidOrgId, ValidProfileId, Guid.Empty, ValidStartDate);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEndDateBeforeStartDate_WhenCreatingOrganizationMember_ThenShouldThrowArgumentException()
    {
        var endDateBeforeStart = ValidStartDate.AddDays(-1);

        var act = () =>
            new OrganizationMember(
                ValidOrgId,
                ValidProfileId,
                ValidRoleId,
                ValidStartDate,
                endDateBeforeStart
            );

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenValidRoleId_WhenChangingRole_ThenShouldUpdateOrganizationMemberRoleId()
    {
        var member = CreateValidMember();
        var newRoleId = Guid.CreateVersion7();

        member.ChangeRole(newRoleId);

        member.OrganizationMemberRoleId.ShouldBe(newRoleId);
    }

    [Test]
    public void GivenEmptyRoleId_WhenChangingRole_ThenShouldThrowArgumentException()
    {
        var member = CreateValidMember();

        var act = () => member.ChangeRole(Guid.Empty);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenEndDateAfterStartDate_WhenDeactivating_ThenShouldSetEndDateAndArchiveStatus()
    {
        var member = CreateValidMember();
        var endDate = ValidStartDate.AddMonths(6);

        member.Deactivate(endDate);

        member.EndDate.ShouldBe(endDate);
        member.Status.ShouldBe(OrganizationStatus.Archived);
    }

    [Test]
    public void GivenEndDateEqualToStartDate_WhenDeactivating_ThenShouldSucceed()
    {
        var member = CreateValidMember();

        member.Deactivate(ValidStartDate);

        member.EndDate.ShouldBe(ValidStartDate);
        member.Status.ShouldBe(OrganizationStatus.Archived);
    }

    [Test]
    public void GivenEndDateBeforeStartDate_WhenDeactivating_ThenShouldThrowArgumentException()
    {
        var member = CreateValidMember();
        var endDateBeforeStart = ValidStartDate.AddDays(-1);

        var act = () => member.Deactivate(endDateBeforeStart);

        act.ShouldThrow<ArgumentException>();
    }

    [Test]
    public void GivenActiveMember_WhenDeleting_ThenShouldMarkAsDeletedAndSetDeletedStatus()
    {
        var member = CreateValidMember();

        member.Delete();

        member.IsDeleted.ShouldBeTrue();
        member.Status.ShouldBe(OrganizationStatus.Deleted);
    }
}
