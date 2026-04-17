namespace Edvantix.Organizational.UnitTests.Features.Organizations.List;

public sealed class GetOrganizationsQueryHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repoMock = new();
    private readonly Mock<IMapper<Organization, OrganizationDto>> _mapperMock = new();
    private readonly GetOrganizationsQueryHandler _handler;

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    public GetOrganizationsQueryHandlerTests()
    {
        _handler = new(_repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenOrganizationsExist_WhenHandling_ThenShouldReturnPagedResult()
    {
        var org = CreateOrganization();
        var dto = CreateDto(org.Id);
        var query = new GetOrganizationsQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([org]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map(org)).Returns(dto);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(1);
        result.TotalItems.ShouldBe(1);
        result.PageIndex.ShouldBe(1);
        result.PageSize.ShouldBe(10);
    }

    [Test]
    public async Task GivenNoOrganizations_WhenHandling_ThenShouldReturnEmptyPagedResult()
    {
        var query = new GetOrganizationsQuery();

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.ShouldBeEmpty();
        result.TotalItems.ShouldBe(0);
    }

    [Test]
    public async Task GivenPageIndexBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetOrganizationsQuery(PageIndex: -5, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageIndex.ShouldBe(1);
    }

    [Test]
    public async Task GivenPageSizeAbove100_WhenHandling_ThenShouldClampTo100()
    {
        var query = new GetOrganizationsQuery(PageIndex: 1, PageSize: 500);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(100);
    }

    [Test]
    public async Task GivenPageSizeBelowOne_WhenHandling_ThenShouldClampToOne()
    {
        var query = new GetOrganizationsQuery(PageIndex: 1, PageSize: 0);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(0);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.PageSize.ShouldBe(1);
    }

    [Test]
    public async Task GivenMultipleOrganizations_WhenHandling_ThenShouldMapEachOrganization()
    {
        var org1 = CreateOrganization();
        var org2 = CreateOrganization();
        var dto1 = CreateDto(org1.Id);
        var dto2 = CreateDto(org2.Id);
        var query = new GetOrganizationsQuery(PageIndex: 1, PageSize: 10);

        _repoMock
            .Setup(r =>
                r.ListAsync(It.IsAny<ISpecification<Organization>>(), It.IsAny<CancellationToken>())
            )
            .ReturnsAsync([org1, org2]);
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<Organization>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(2);
        _mapperMock.Setup(m => m.Map(org1)).Returns(dto1);
        _mapperMock.Setup(m => m.Map(org2)).Returns(dto2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Count.ShouldBe(2);
        result.TotalItems.ShouldBe(2);
        _mapperMock.Verify(m => m.Map(It.IsAny<Organization>()), Times.Exactly(2));
    }

    private static Organization CreateOrganization() =>
        new(
            "ООО Тестовая Компания",
            isLegalEntity: true,
            new DateOnly(2020, 1, 15),
            LegalForm.Llc,
            ValidCountryId,
            ValidCurrencyId,
            OrganizationType.PrivateEducationalCenter
        );

    private static OrganizationDto CreateDto(Guid id) =>
        new(
            id,
            "ООО Тестовая Компания",
            null,
            OrganizationType.PrivateEducationalCenter,
            OrganizationStatus.Active,
            true
        );
}
