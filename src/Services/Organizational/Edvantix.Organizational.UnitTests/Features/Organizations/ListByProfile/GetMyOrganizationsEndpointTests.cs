namespace Edvantix.Organizational.UnitTests.Features.Organizations.ListByProfile;

public sealed class GetMyOrganizationsEndpointTests
{
    private readonly GetMyOrganizationsEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    [Test]
    public async Task GivenOrganizationsExist_WhenHandling_ThenShouldReturnOkWithList()
    {
        var dto = CreateDto();
        IReadOnlyList<OrganizationWithRoleDto> list = [dto];

        _senderMock
            .Setup(s => s.Send(It.IsAny<GetMyOrganizationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<OrganizationWithRoleDto>>>();
        result.Value!.ShouldHaveSingleItem();
    }

    [Test]
    public async Task GivenNoOrganizations_WhenHandling_ThenShouldReturnOkWithEmptyList()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetMyOrganizationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<OrganizationWithRoleDto>)[]);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        result.ShouldBeOfType<Ok<IReadOnlyList<OrganizationWithRoleDto>>>();
        result.Value!.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenHandling_WhenCalled_ThenShouldSendGetMyOrganizationsQuery()
    {
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetMyOrganizationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IReadOnlyList<OrganizationWithRoleDto>)[]);

        await _endpoint.HandleAsync(_senderMock.Object);

        _senderMock.Verify(
            s => s.Send(It.IsAny<GetMyOrganizationsQuery>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenOrganizationsExist_WhenHandling_ThenShouldReturnCorrectDtoValues()
    {
        var dto = CreateDto();
        IReadOnlyList<OrganizationWithRoleDto> list = [dto];

        _senderMock
            .Setup(s => s.Send(It.IsAny<GetMyOrganizationsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(list);

        var result = await _endpoint.HandleAsync(_senderMock.Object);

        var item = result.Value![0];
        item.Id.ShouldBe(dto.Id);
        item.RoleCode.ShouldBe(dto.RoleCode);
        item.FullLegalName.ShouldBe(dto.FullLegalName);
    }

    private static OrganizationWithRoleDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "ООО Тест",
            null,
            OrganizationType.ItSchool,
            OrganizationStatus.Active,
            true,
            "teacher",
            "Преподаватель"
        );
}
