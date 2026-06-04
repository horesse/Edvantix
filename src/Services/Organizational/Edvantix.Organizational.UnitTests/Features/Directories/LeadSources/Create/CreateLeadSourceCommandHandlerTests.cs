namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Create;

public sealed class CreateLeadSourceCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<ILeadSourceRepository> _repoMock = new();
    private readonly Mock<IMapper<LeadSource, LeadSourceDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly CreateLeadSourceCommandHandler _handler;

    public CreateLeadSourceCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<LeadSource>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveAndReturnDto()
    {
        var expectedDto = CreateDto();
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, "utm_insta");
        _mapperMock.Setup(m => m.Map(It.IsAny<LeadSource>())).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedDto);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenLeadSourceShouldBelongToCurrentOrganization()
    {
        LeadSource? capturedSource = null;
        var command = new CreateLeadSourceCommand("Флаер", LeadChannel.Offline, null, Order: 1);
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<LeadSource>(), It.IsAny<CancellationToken>()))
            .Callback<LeadSource, CancellationToken>((src, _) => capturedSource = src)
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<LeadSource>())).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        capturedSource.ShouldNotBeNull();
        capturedSource!.OrganizationId.ShouldBe(_orgId);
        capturedSource.Name.ShouldBe("Флаер");
        capturedSource.Channel.ShouldBe(LeadChannel.Offline);
        capturedSource.UtmTag.ShouldBeNull();
        capturedSource.IsDeleted.ShouldBeFalse();
    }

    private static LeadSourceDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Инстаграм",
            LeadChannel.Online,
            "utm_insta",
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
