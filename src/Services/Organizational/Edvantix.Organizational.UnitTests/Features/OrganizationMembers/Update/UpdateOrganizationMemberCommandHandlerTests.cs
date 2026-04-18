using Edvantix.Chassis.Security.Tenant;

namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Update;

public sealed class UpdateOrganizationMemberCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly UpdateOrganizationMemberCommandHandler _handler;

    public UpdateOrganizationMemberCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenExistingMember_WhenChangingRole_ThenShouldSaveChanges()
    {
        var newRoleId = Guid.CreateVersion7();
        var member = CreateMember(_organizationId);
        var command = new UpdateOrganizationMemberCommand(member.Id, newRoleId);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        member.OrganizationMemberRoleId.ShouldBe(newRoleId);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMemberNotFound_WhenChangingRole_ThenShouldThrowNotFoundException()
    {
        var memberId = Guid.CreateVersion7();
        var command = new UpdateOrganizationMemberCommand(memberId, Guid.CreateVersion7());

        _repoMock
            .Setup(r => r.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMember?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenMemberFromDifferentOrganization_WhenChangingRole_ThenShouldThrowNotFoundException()
    {
        var member = CreateMember(Guid.CreateVersion7());
        var command = new UpdateOrganizationMemberCommand(member.Id, Guid.CreateVersion7());

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    private static OrganizationMember CreateMember(Guid orgId) =>
        new(orgId, Guid.CreateVersion7(), Guid.CreateVersion7(), new DateOnly(2025, 1, 1));
}
