namespace Edvantix.Organizational.UnitTests.Features.OrganizationMembers.Delete;

public sealed class DeleteOrganizationMemberCommandHandlerTests
{
    private readonly Mock<IOrganizationMemberRepository> _repoMock = new();
    private readonly DeleteOrganizationMemberCommandHandler _handler;

    public DeleteOrganizationMemberCommandHandlerTests()
    {
        _handler = new(_repoMock.Object);
    }

    [Test]
    public async Task GivenExistingMember_WhenDeleting_ThenShouldSoftDeleteAndSave()
    {
        var orgId = Guid.CreateVersion7();
        var member = CreateMember(orgId);
        var command = new DeleteOrganizationMemberCommand(orgId, member.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(member.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(member);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        await _handler.Handle(command, CancellationToken.None);

        member.IsDeleted.ShouldBeTrue();
        member.Status.ShouldBe(OrganizationStatus.Deleted);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenMemberNotFound_WhenDeleting_ThenShouldThrowNotFoundException()
    {
        var orgId = Guid.CreateVersion7();
        var memberId = Guid.CreateVersion7();
        var command = new DeleteOrganizationMemberCommand(orgId, memberId);

        _repoMock
            .Setup(r => r.GetByIdAsync(memberId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((OrganizationMember?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(command, CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenMemberFromDifferentOrganization_WhenDeleting_ThenShouldThrowNotFoundException()
    {
        var requestOrgId = Guid.CreateVersion7();
        var actualOrgId = Guid.CreateVersion7();
        var member = CreateMember(actualOrgId);
        var command = new DeleteOrganizationMemberCommand(requestOrgId, member.Id);

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
