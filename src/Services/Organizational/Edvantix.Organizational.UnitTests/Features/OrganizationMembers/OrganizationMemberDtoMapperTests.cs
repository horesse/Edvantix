namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers;

public sealed class OrganizationMemberDtoMapperTests
{
    private readonly OrganizationMemberDtoMapper _mapper = new();

    [Test]
    public void GivenActiveMember_WhenMapping_ThenShouldMapAllProperties()
    {
        var organizationId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();
        var roleId = Guid.CreateVersion7();
        var startDate = new DateOnly(2025, 1, 1);

        var member = new OrganizationMember(organizationId, profileId, roleId, startDate);

        var dto = _mapper.Map(member);

        dto.Id.ShouldBe(member.Id);
        dto.OrganizationId.ShouldBe(organizationId);
        dto.ProfileId.ShouldBe(profileId);
        dto.OrganizationMemberRoleId.ShouldBe(roleId);
        dto.StartDate.ShouldBe(startDate);
        dto.Status.ShouldBe(OrganizationStatus.Active);
        dto.EndDate.ShouldBeNull();
    }

    [Test]
    public void GivenMemberWithEndDate_WhenMapping_ThenEndDateShouldBeMapped()
    {
        var startDate = new DateOnly(2025, 1, 1);
        var endDate = new DateOnly(2025, 12, 31);

        var member = new OrganizationMember(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            startDate,
            endDate
        );

        var dto = _mapper.Map(member);

        dto.StartDate.ShouldBe(startDate);
        dto.EndDate.ShouldBe(endDate);
    }

    [Test]
    public void GivenDeactivatedMember_WhenMapping_ThenStatusShouldBeArchived()
    {
        var member = new OrganizationMember(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2025, 1, 1)
        );
        member.Deactivate(new DateOnly(2025, 6, 30));

        var dto = _mapper.Map(member);

        dto.Status.ShouldBe(OrganizationStatus.Archived);
        dto.EndDate.ShouldBe(new DateOnly(2025, 6, 30));
    }

    [Test]
    public void GivenDeletedMember_WhenMapping_ThenStatusShouldBeDeleted()
    {
        var member = new OrganizationMember(
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            new DateOnly(2025, 1, 1)
        );
        member.Delete();

        var dto = _mapper.Map(member);

        dto.Status.ShouldBe(OrganizationStatus.Deleted);
    }
}
