using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Create;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Create;

public sealed class CreatePaymentMethodCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Mock<IMapper<PaymentMethod, PaymentMethodDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly CreatePaymentMethodCommandHandler _handler;

    public CreatePaymentMethodCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<PaymentMethod>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _repoMock
            .Setup(r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(
            _tenantMock.Object,
            _claimsMock.Object,
            _repoMock.Object,
            _mapperMock.Object
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenShouldSaveAndReturnDto()
    {
        var expectedDto = CreateDto();
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false);
        _mapperMock.Setup(m => m.Map(It.IsAny<PaymentMethod>())).Returns(expectedDto);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldBe(expectedDto);
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenValidCommand_WhenHandling_ThenPaymentMethodShouldBelongToCurrentOrganization()
    {
        PaymentMethod? capturedMethod = null;
        var command = new CreatePaymentMethodCommand(
            "Рассрочка",
            "installment",
            false,
            true,
            Order: 1
        );
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<PaymentMethod>(), It.IsAny<CancellationToken>()))
            .Callback<PaymentMethod, CancellationToken>((pm, _) => capturedMethod = pm)
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map(It.IsAny<PaymentMethod>())).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        capturedMethod.ShouldNotBeNull();
        capturedMethod!.OrganizationId.ShouldBe(_orgId);
        capturedMethod.Name.ShouldBe("Рассрочка");
        capturedMethod.Code.ShouldBe("installment");
        capturedMethod.IsCashless.ShouldBeFalse();
        capturedMethod.RequiresContract.ShouldBeTrue();
        capturedMethod.IsDeleted.ShouldBeFalse();
    }

    private static PaymentMethodDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
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
