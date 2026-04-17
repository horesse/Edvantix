namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Update;

public sealed class UpdateOrganizationMemberCommandHandlerTests
{
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly UpdateOrganizationMemberCommandHandler _handler;

    public UpdateOrganizationMemberCommandHandlerTests()
    {
        _handler = new(_repoMock.Object);
    }

    [Test]
    public async Task GivenExistingMember_WhenChangingRole_ThenShouldSaveChanges()
    {
        var orgId = Guid.CreateVersion7();
        var newRoleId = Guid.CreateVersion7();
        var member = CreateMember(orgId);
        var command = new UpdateOrganizationMemberCommand(orgId, member.Id, newRoleId);

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
        var orgId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();
        var command = new UpdateOrganizationMemberCommand(orgId, memberId, Guid.CreateVersion7());

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
        var requestOrgId = Guid.CreateVersion7();
        var actualOrgId = Guid.CreateVersion7();
        var member = CreateMember(actualOrgId);
        var command = new UpdateOrganizationMemberCommand(
            requestOrgId,
            member.Id,
            Guid.CreateVersion7()
        );

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
