using Edvantix.Organizational.Domain.AggregatesModel.LevelAggregate;
using Edvantix.Organizational.Features.Directories.Levels.GetById;
using Edvantix.Organizational.Grpc.Services.Groups;

namespace Edvantix.Organizational.UnitTests.Features.Directories.Levels.GetById;

public sealed class GetLevelDirectoryByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ILevelRepository> _repoMock = new();
    private readonly Mock<IGroupsUsageService> _usageMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly GetLevelDirectoryByIdQueryHandler _handler;

    public GetLevelDirectoryByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _usageMock
            .Setup(s =>
                s.CountByLevelIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, int>());
        _handler = new(_tenantMock.Object, _repoMock.Object, _usageMock.Object);
    }

    [Test]
    public async Task GivenExistingLevelWithNoGroups_WhenGettingById_ThenShouldReturnDtoWithZeroUsage()
    {
        var level = CreateLevel();
        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);

        var result = await _handler.Handle(
            new GetLevelDirectoryByIdQuery(level.Id),
            CancellationToken.None
        );

        result.Id.ShouldBe(level.Id);
        result.Name.ShouldBe(level.Name);
        result.Usage.ShouldHaveSingleItem();
        result.Usage[0].Label.ShouldBe("Группы");
        result.Usage[0].Count.ShouldBe(0);
    }

    [Test]
    public async Task GivenExistingLevelWithGroups_WhenGettingById_ThenShouldReturnDtoWithGroupCount()
    {
        var level = CreateLevel();
        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);
        _usageMock
            .Setup(s =>
                s.CountByLevelIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(new Dictionary<Guid, int> { [level.Id] = 7 });

        var result = await _handler.Handle(
            new GetLevelDirectoryByIdQuery(level.Id),
            CancellationToken.None
        );

        result.Usage.ShouldHaveSingleItem();
        result.Usage[0].Count.ShouldBe(7);
    }

    [Test]
    public async Task GivenLevelNotFound_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetLevelDirectoryByIdQuery(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenLevelFromDifferentOrganization_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var level = new Level(
            Guid.CreateVersion7(),
            LevelCode.From("A1"),
            "Чужой уровень",
            null,
            LevelTone.Blue,
            1
        );
        _repoMock
            .Setup(r => r.GetByIdAsync(level.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(new GetLevelDirectoryByIdQuery(level.Id), CancellationToken.None)
                .AsTask()
        );
    }

    private Level CreateLevel() =>
        new(_orgId, LevelCode.From("B2"), "Средний", null, LevelTone.Green, 2);
}
