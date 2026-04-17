using Edvantix.Chassis.Caching;

namespace Edvantix.Organizational.UnitTests.Features.Organizations.Get;

public sealed class GetOrganizationQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repoMock = new();
    private readonly Mock<IMapper<Organization, OrganizationDetailDto>> _mapperMock = new();
    private readonly Mock<IHybridCache> _cacheMock = new();
    private readonly GetOrganizationQueryHandler _handler;

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    public GetOrganizationQueryHandlerTests()
    {
        _cacheMock
            .Setup(c =>
                c.GetOrCreateAsync(
                    It.IsAny<string>(),
                    It.IsAny<Func<CancellationToken, ValueTask<Organization>>>(),
                    It.IsAny<IEnumerable<string>?>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(
                (
                    string _,
                    Func<CancellationToken, ValueTask<Organization>> factory,
                    IEnumerable<string>? _,
                    CancellationToken ct
                ) => factory(ct)
            );

        _handler = new(_cacheMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingOrganization_WhenQuerying_ThenShouldReturnDto()
    {
        var orgId = Guid.CreateVersion7();
        var org = CreateOrganization(orgId);
        var dto = CreateDto(orgId);

        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(org);
        _mapperMock.Setup(m => m.Map(org)).Returns(dto);

        var result = await _handler.Handle(new GetOrganizationQuery(orgId), CancellationToken.None);

        result.ShouldBe(dto);
        result.Id.ShouldBe(orgId);
    }

    [Test]
    public async Task GivenExistingOrganization_WhenQuerying_ThenShouldCallMapper()
    {
        var orgId = Guid.CreateVersion7();
        var org = CreateOrganization(orgId);

        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(org);
        _mapperMock.Setup(m => m.Map(org)).Returns(CreateDto(orgId));

        await _handler.Handle(new GetOrganizationQuery(orgId), CancellationToken.None);

        _mapperMock.Verify(m => m.Map(org), Times.Once);
    }

    [Test]
    public async Task GivenOrganizationNotFound_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        var orgId = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetOrganizationQuery(orgId), CancellationToken.None).AsTask()
        );
    }

    private static Organization CreateOrganization(Guid id) =>
        new Organization(
            "ООО Тестовая Компания",
            isLegalEntity: true,
            new DateOnly(2020, 1, 15),
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter
        )
        {
            Id = id,
        };

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
}
