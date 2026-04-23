namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers;

public sealed class OrganizationMemberDtoMapperTests
{
    private readonly OrganizationMemberDtoMapper _mapper = new();

    [Test]
    public void GivenActiveMember_WhenMapping_ThenShouldMapCoreProperties()
    {
        var organizationId = Guid.CreateVersion7();
        var profileId = Guid.CreateVersion7();

        var member = new OrganizationMember(
            organizationId,
            profileId,
            Guid.CreateVersion7(),
            new DateOnly(2025, 1, 1)
        );

        var dto = _mapper.Map(member);

        dto.Id.ShouldBe(member.Id);
        dto.ProfileId.ShouldBe(profileId);
        dto.Status.ShouldBe(OrganizationStatus.Active);
        dto.Role.ShouldBe(string.Empty); // Role — nav property, загружается через EF Include
        dto.FullName.ShouldBe(string.Empty);
        dto.AvatarUrl.ShouldBeNull();
        dto.LastActivity.ShouldBeNull();
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
