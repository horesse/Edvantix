using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.List;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.List;

public sealed class ListPaymentMethodsQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Mock<IMapper<PaymentMethod, PaymentMethodListItemDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly ListPaymentMethodsQueryHandler _handler;

    public ListPaymentMethodsQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenPaymentMethods_WhenListing_ThenShouldReturnPagedResult()
    {
        var methods = new List<PaymentMethod>
        {
            new(_orgId, "Карта", "card", true, false),
            new(_orgId, "Наличные", "cash", false, false),
        };
        SetupList(methods);
        SetupCount(2);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<PaymentMethod>>()))
            .Returns(methods.Select(MapToDto).ToList());

        var result = await _handler.Handle(new ListPaymentMethodsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(2);
        result.Count.ShouldBe(2);
    }

    [Test]
    public async Task GivenNoPaymentMethods_WhenListing_ThenShouldReturnEmptyPagedResult()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<PaymentMethod>>()))
            .Returns(Array.Empty<PaymentMethodListItemDto>());

        var result = await _handler.Handle(new ListPaymentMethodsQuery(), CancellationToken.None);

        result.TotalItems.ShouldBe(0);
        result.ShouldBeEmpty();
    }

    [Test]
    public async Task GivenPaginationParams_WhenListing_ThenShouldPassThemToResult()
    {
        SetupList([]);
        SetupCount(100);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<PaymentMethod>>()))
            .Returns(Array.Empty<PaymentMethodListItemDto>());

        var result = await _handler.Handle(
            new ListPaymentMethodsQuery(Page: 3, PageSize: 10),
            CancellationToken.None
        );

        result.PageIndex.ShouldBe(3);
        result.PageSize.ShouldBe(10);
        result.TotalItems.ShouldBe(100);
    }

    [Test]
    public async Task GivenSearchTerm_WhenListing_ThenShouldCallBothSpecifications()
    {
        SetupList([]);
        SetupCount(0);
        _mapperMock
            .Setup(m => m.Map(It.IsAny<IReadOnlyCollection<PaymentMethod>>()))
            .Returns(Array.Empty<PaymentMethodListItemDto>());

        await _handler.Handle(new ListPaymentMethodsQuery(Search: "Кар"), CancellationToken.None);

        _repoMock.Verify(
            r =>
                r.ListAsync(
                    It.IsAny<ISpecification<PaymentMethod>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
    }

    private void SetupList(IReadOnlyList<PaymentMethod> items) =>
        _repoMock
            .Setup(r =>
                r.ListAsync(
                    It.IsAny<ISpecification<PaymentMethod>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(items);

    private void SetupCount(int count) =>
        _repoMock
            .Setup(r =>
                r.CountAsync(
                    It.IsAny<ISpecification<PaymentMethod>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(count);

    private static PaymentMethodListItemDto MapToDto(PaymentMethod pm) =>
        new(pm.Id, pm.Name, pm.Code, pm.IsCashless, pm.RequiresContract, pm.IsDeleted, pm.Order);
}
