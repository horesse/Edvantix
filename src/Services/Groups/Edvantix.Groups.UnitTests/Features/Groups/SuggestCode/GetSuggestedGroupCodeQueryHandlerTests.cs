namespace Edvantix.Groups.UnitTests.Features.Groups.SuggestCode;

public sealed class GetSuggestedGroupCodeQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Mock<ILevelRepository> _levelRepoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetSuggestedGroupCodeQueryHandler _handler;

    public GetSuggestedGroupCodeQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _levelRepoMock.Object);
    }

    [Test]
    public async Task GivenNoExistingGroups_WhenSuggesting_ThenShouldReturnFirstCode()
    {
        var levelId = SetupLevel("B1");
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery(levelId),
            CancellationToken.None
        );

        result.ShouldBe("B1-01");
    }

    [Test]
    public async Task GivenExistingGroupsWithSameLevel_WhenSuggesting_ThenShouldIncrementNumber()
    {
        var levelId = SetupLevel("B1");
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(["B1-01", "B1-02", "B1-03"]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery(levelId),
            CancellationToken.None
        );

        result.ShouldBe("B1-04");
    }

    [Test]
    public async Task GivenExistingGroupsWithDifferentLevel_WhenSuggesting_ThenShouldReturnFirstCode()
    {
        var levelId = SetupLevel("B1");
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(["A1-01", "A1-02"]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery(levelId),
            CancellationToken.None
        );

        result.ShouldBe("B1-01");
    }

    [Test]
    public async Task GivenSingleDigitMaxNumber_WhenSuggesting_ThenShouldZeroPad()
    {
        var levelId = SetupLevel("B1");
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(["B1-09"]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery(levelId),
            CancellationToken.None
        );

        result.ShouldBe("B1-10");
    }

    [Test]
    public async Task GivenLevelNotFound_WhenSuggesting_ThenShouldThrowNotFoundException()
    {
        var unknownLevelId = Guid.CreateVersion7();
        _levelRepoMock
            .Setup(r => r.GetByIdAsync(unknownLevelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Level?)null);

        var act = async () =>
            await _handler.Handle(
                new GetSuggestedGroupCodeQuery(unknownLevelId),
                CancellationToken.None
            );

        await act.ShouldThrowAsync<NotFoundException>();
    }

    private Guid SetupLevel(string code)
    {
        var levelId = Guid.CreateVersion7();
        var level = new Level(
            _organizationId,
            LevelCode.From(code),
            $"Level {code}",
            null,
            LevelTone.Blue,
            10
        );
        _levelRepoMock
            .Setup(r => r.GetByIdAsync(levelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(level);
        return levelId;
    }
}
