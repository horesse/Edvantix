namespace Edvantix.Groups.UnitTests.Features.Groups.SuggestCode;

public sealed class GetSuggestedGroupCodeQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IGroupRepository> _repoMock = new();
    private readonly Guid _organizationId = Guid.CreateVersion7();
    private readonly GetSuggestedGroupCodeQueryHandler _handler;

    public GetSuggestedGroupCodeQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);

        // Level хранится в Organizational-сервисе; Groups использует строковый LevelCode
        // напрямую (кросс-сервисная мягкая ссылка). ILevelRepository больше не нужен.
        _handler = new(_tenantMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenNoExistingGroups_WhenSuggesting_ThenShouldReturnFirstCode()
    {
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery("B1"),
            CancellationToken.None
        );

        result.ShouldBe("B1-01");
    }

    [Test]
    public async Task GivenExistingGroupsWithSameLevel_WhenSuggesting_ThenShouldIncrementNumber()
    {
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(["B1-01", "B1-02", "B1-03"]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery("B1"),
            CancellationToken.None
        );

        result.ShouldBe("B1-04");
    }

    [Test]
    public async Task GivenExistingGroupsWithDifferentLevel_WhenSuggesting_ThenShouldReturnFirstCode()
    {
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(["A1-01", "A1-02"]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery("B1"),
            CancellationToken.None
        );

        result.ShouldBe("B1-01");
    }

    [Test]
    public async Task GivenSingleDigitMaxNumber_WhenSuggesting_ThenShouldZeroPad()
    {
        _repoMock
            .Setup(r =>
                r.GetCodesByOrganizationAsync(_organizationId, It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(["B1-09"]);

        var result = await _handler.Handle(
            new GetSuggestedGroupCodeQuery("B1"),
            CancellationToken.None
        );

        result.ShouldBe("B1-10");
    }
}
