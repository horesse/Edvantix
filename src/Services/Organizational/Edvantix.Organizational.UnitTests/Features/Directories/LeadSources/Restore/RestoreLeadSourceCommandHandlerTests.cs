namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Restore;

public sealed class RestoreLeadSourceCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<ILeadSourceRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly RestoreLeadSourceCommandHandler _handler;

    public RestoreLeadSourceCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _claimsMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenArchivedSource_WhenRestoring_ThenShouldSetIsArchivedFalseAndSave()
    {
        var source = CreateLeadSource(_orgId);
        source.Archive(_profileId);
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        await _handler.Handle(new RestoreLeadSourceCommand(source.Id), CancellationToken.None);

        source.IsDeleted.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenActiveSource_WhenRestoring_ThenShouldBeIdempotent()
    {
        var source = CreateLeadSource(_orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        await _handler.Handle(new RestoreLeadSourceCommand(source.Id), CancellationToken.None);

        source.IsDeleted.ShouldBeFalse();
    }

    [Test]
    public async Task GivenSourceNotFound_WhenRestoring_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LeadSource?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new RestoreLeadSourceCommand(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenSourceFromDifferentOrganization_WhenRestoring_ThenShouldThrowNotFoundException()
    {
        var source = CreateLeadSource(Guid.CreateVersion7());
        source.Archive(_profileId);
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new RestoreLeadSourceCommand(source.Id), CancellationToken.None)
                .AsTask()
        );
    }

    private static LeadSource CreateLeadSource(Guid orgId) =>
        new(orgId, "Инстаграм", LeadChannel.Online, null);
}
