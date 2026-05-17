namespace Edvantix.Groups.UnitTests.Features.Levels.Get;

public sealed class GetLevelByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Mock<IMapper<Level, LevelDto>> _mapperMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetLevelByIdQueryHandler _handler;

    public GetLevelByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingLevel_WhenHandling_ThenReturnsMappedDto()
    {
        var level = CreateLevel(_organizationId);
        var expectedDto = CreateDto(level.Id);

        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);
        _mapperMock.Setup(m => m.Map(level)).Returns(expectedDto);

        var result = await _handler.Handle(new GetLevelByIdQuery(level.Id), CancellationToken.None);

        result.ShouldBe(expectedDto);
        _mapperMock.Verify(m => m.Map(level), Times.Once);
    }

    [Test]
    public async Task GivenLevelNotFound_WhenHandling_ThenThrowsNotFoundException()
    {
        var id = Guid.CreateVersion7();

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        var act = async () =>
            await _handler.Handle(new GetLevelByIdQuery(id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task GivenLevelFromOtherOrg_WhenHandling_ThenThrowsNotFoundException()
    {
        var level = CreateLevel(Guid.CreateVersion7());

        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);

        var act = async () =>
            await _handler.Handle(new GetLevelByIdQuery(level.Id), CancellationToken.None);

        await act.ShouldThrowAsync<NotFoundException>();
    }

    private static Level CreateLevel(Guid orgId) =>
        new(orgId, LevelCode.From("B2"), "Intermediate", null, LevelTone.Teal, sortOrder: 2);

    private static LevelDto CreateDto(Guid id) =>
        new(
            id,
            "B2",
            "Intermediate",
            null,
            LevelTone.Teal,
            SortOrder: 2,
            IsActive: true,
            UsageCount: 0
        );
}
