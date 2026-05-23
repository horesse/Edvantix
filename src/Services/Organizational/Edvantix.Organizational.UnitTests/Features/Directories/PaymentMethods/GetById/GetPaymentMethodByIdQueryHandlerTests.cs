using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.GetById;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.GetById;

public sealed class GetPaymentMethodByIdQueryHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Mock<IMapper<PaymentMethod, PaymentMethodDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly GetPaymentMethodByIdQueryHandler _handler;

    public GetPaymentMethodByIdQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _handler = new(_tenantMock.Object, _repoMock.Object, _mapperMock.Object);
    }

    [Test]
    public async Task GivenExistingPaymentMethod_WhenGettingById_ThenShouldReturnDto()
    {
        var pm = CreatePaymentMethod(_orgId);
        var expectedDto = CreateDto(pm.Id);
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);
        _mapperMock.Setup(m => m.Map(pm)).Returns(expectedDto);

        var result = await _handler.Handle(
            new GetPaymentMethodByIdQuery(pm.Id),
            CancellationToken.None
        );

        result.ShouldBe(expectedDto);
    }

    [Test]
    public async Task GivenPaymentMethodNotFound_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetPaymentMethodByIdQuery(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenPaymentMethodFromDifferentOrganization_WhenGettingById_ThenShouldThrowNotFoundException()
    {
        var pm = CreatePaymentMethod(Guid.CreateVersion7());
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetPaymentMethodByIdQuery(pm.Id), CancellationToken.None).AsTask()
        );
    }

    private static PaymentMethod CreatePaymentMethod(Guid orgId) =>
        new(orgId, "Карта", "card", true, false);

    private static PaymentMethodDto CreateDto(Guid id) =>
        new(
            id,
            "Карта",
            "card",
            true,
            false,
            false,
            0,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
