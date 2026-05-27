using Edvantix.Organizational.Features.Directories.PaymentMethods.Reorder;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Reorder;

public sealed class ReorderPaymentMethodsCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ReorderPaymentMethodsCommandHandler _handler;

    public ReorderPaymentMethodsCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _handler = new(_tenantMock.Object, _claimsMock.Object, _repoMock.Object);
    }

    [Test]
    public async Task GivenThreePaymentMethods_WhenReordering_ThenOrderShouldMatchIndexPosition()
    {
        var pm1 = CreatePaymentMethod();
        var pm2 = CreatePaymentMethod();
        var pm3 = CreatePaymentMethod();
        SetupList([pm1, pm2, pm3]);

        await _handler.Handle(
            new ReorderPaymentMethodsCommand([pm3.Id, pm1.Id, pm2.Id]),
            CancellationToken.None
        );

        pm3.Order.ShouldBe(0);
        pm1.Order.ShouldBe(1);
        pm2.Order.ShouldBe(2);
    }

    [Test]
    public async Task GivenReordering_WhenCalled_ThenShouldSaveOnce()
    {
        var pm = CreatePaymentMethod();
        SetupList([pm]);

        await _handler.Handle(new ReorderPaymentMethodsCommand([pm.Id]), CancellationToken.None);

        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenUnknownId_WhenReordering_ThenShouldBeIgnored()
    {
        var pm = CreatePaymentMethod();
        SetupList([pm]);
        var unknownId = Guid.CreateVersion7();

        await _handler.Handle(
            new ReorderPaymentMethodsCommand([unknownId, pm.Id]),
            CancellationToken.None
        );

        pm.Order.ShouldBe(1);
    }

    private PaymentMethod CreatePaymentMethod()
    {
        var pm = new PaymentMethod(
            _orgId,
            "Карта",
            "CARD",
            isCashless: true,
            requiresContract: false
        );
        pm.Id = Guid.CreateVersion7();
        return pm;
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
}
