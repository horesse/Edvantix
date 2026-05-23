namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.GetById;

public sealed class GetLeadSourceByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILeadSourceRepository> _repoMock = new();
    private readonly Mock<IMapper<LeadSource, LeadSourceDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly GetLeadSourceByIdQueryHandler _handler;

    public GetLeadSourceByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingSource_WhenGettingById_ThenShouldReturnDto()
    {
        var source = CreateLeadSource(_orgId);
        var expectedDto = CreateDto(source.Id);
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);
        _mapperMock.Setup(m => m.Map(source)).Returns(expectedDto);

        var result = await _handler.Handle(
            new GetLeadSourceByIdQuery(source.Id),
            CancellationToken.None
        );

        result.ShouldBe(expectedDto);
    }

    [Test]
    public async Task GivenSourceNotFound_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LeadSource?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetLeadSourceByIdQuery(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenSourceFromDifferentOrganization_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var source = CreateLeadSource(Guid.CreateVersion7());
        _repoMock
            .Setup(r => r.GetByIdAsync(source.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(source);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetLeadSourceByIdQuery(source.Id), CancellationToken.None).AsTask()
        );
    }

    private static LeadSource CreateLeadSource(Guid orgId) =>
        new(orgId, "Инстаграм", LeadChannel.Online, null);

    private static LeadSourceDto CreateDto(Guid id) =>
        new(
            id,
            "Инстаграм",
            LeadChannel.Online,
            null,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
