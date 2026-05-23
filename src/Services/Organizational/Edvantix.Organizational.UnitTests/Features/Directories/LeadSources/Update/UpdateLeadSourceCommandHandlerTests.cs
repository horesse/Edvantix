namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Update;

public sealed class UpdateLeadSourceCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<ILeadSourceRepository> _repoMock = new();
    private readonly Mock<IMapper<LeadSource, LeadSourceDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly UpdateLeadSourceCommandHandler _handler;

    public UpdateLeadSourceCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(
            _tenantMock.Object,
            _claimsMock.Object,
            _repoMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task GivenExistingSource_WhenUpdating_ThenShouldUpdateAndSave()
    {
        var source = CreateLeadSource(_orgId);
        var command = new UpdateLeadSourceCommand(
            source.Id,
            "ВКонтакте",
            LeadChannel.Online,
            "utm_vk",
            2
        );
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);
        _mapperMock.Setup(m => m.Map(source)).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        source.Name.ShouldBe("ВКонтакте");
        source.Channel.ShouldBe(LeadChannel.Online);
        source.UtmTag.ShouldBe("utm_vk");
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenSourceNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LeadSource?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdateLeadSourceCommand(id, "Источник", LeadChannel.Direct, null),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenSourceFromDifferentOrganization_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var source = CreateLeadSource(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdateLeadSourceCommand(source.Id, "Источник", LeadChannel.Direct, null),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private static LeadSource CreateLeadSource(Guid orgId) =>
        new(orgId, "Инстаграм", LeadChannel.Online, null);

    private static LeadSourceDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "ВКонтакте",
            LeadChannel.Online,
            "utm_vk",
            false,
            2,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
