namespace Edvantix.Organizational.UnitTests.Features.Organizations.Get;

public sealed class GetOrganizationEndpointTests
{
    private readonly GetOrganizationEndpoint _endpoint = new();
    private readonly Mock<ISender> _senderMock = new();

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    private static OrganizationDetailDto CreateDto(Guid id) =>
        new(
            id,
            "ООО Тестовая Компания",
            null,
            true,
            new DateOnly(2020, 1, 15),
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter,
            OrganizationStatus.Active,
            []
        );

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldSendQueryWithCorrectId()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s =>
                s.Send(It.Is<GetOrganizationQuery>(q => q.Id == id), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync(CreateDto(id));

        await _endpoint.HandleAsync(id, _senderMock.Object);

        _senderMock.Verify(
            s =>
                s.Send(It.Is<GetOrganizationQuery>(q => q.Id == id), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidId_WhenHandling_ThenShouldReturnOkWithDto()
    {
        var id = Guid.CreateVersion7();
        var dto = CreateDto(id);
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetOrganizationQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dto);

        var result = await _endpoint.HandleAsync(id, _senderMock.Object);

        result.ShouldBeOfType<Ok<OrganizationDetailDto>>();
        result.Value.ShouldBe(dto);
        result.Value!.Id.ShouldBe(id);
    }

    [Test]
    public async Task GivenExceptionFromSender_WhenHandling_ThenShouldPropagateException()
    {
        var id = Guid.CreateVersion7();
        _senderMock
            .Setup(s => s.Send(It.IsAny<GetOrganizationQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(NotFoundException.For<Organization>(id));

        var act = async () => await _endpoint.HandleAsync(id, _senderMock.Object);

        await act.ShouldThrowAsync<NotFoundException>();
    }
}
