using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Update;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Update;

public sealed class UpdatePaymentMethodCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Mock<IMapper<PaymentMethod, PaymentMethodDto>> _mapperMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly UpdatePaymentMethodCommandHandler _handler;

    public UpdatePaymentMethodCommandHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);
        _claimsMock
            .Setup(c => c.FindFirst(It.IsAny<string>()))
            .Returns(new System.Security.Claims.Claim("sub", _profileId.ToString("D")));
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
    public async Task GivenExistingPaymentMethod_WhenUpdating_ThenShouldUpdateAndSave()
    {
        var pm = CreatePaymentMethod(_orgId);
        var command = new UpdatePaymentMethodCommand(pm.Id, "Перевод", "transfer", true, false, 2);
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);
        _mapperMock.Setup(m => m.Map(pm)).Returns(CreateDto());

        await _handler.Handle(command, CancellationToken.None);

        pm.Name.ShouldBe("Перевод");
        pm.Code.ShouldBe("transfer");
        pm.IsCashless.ShouldBeTrue();
        pm.RequiresContract.ShouldBeFalse();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenPaymentMethodNotFound_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdatePaymentMethodCommand(id, "Карта", "card", true, false),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    [Test]
    public async Task GivenPaymentMethodFromDifferentOrganization_WhenUpdating_ThenShouldThrowNotFoundException()
    {
        var pm = CreatePaymentMethod(Guid.CreateVersion7());
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler
                .Handle(
                    new UpdatePaymentMethodCommand(pm.Id, "Карта", "card", true, false),
                    CancellationToken.None
                )
                .AsTask()
        );
    }

    private static PaymentMethod CreatePaymentMethod(Guid orgId) =>
        new(orgId, "Карта", "card", true, false);

    private static PaymentMethodDto CreateDto() =>
        new(
            Guid.CreateVersion7(),
            "Перевод",
            "transfer",
            true,
            false,
            false,
            2,
            Guid.CreateVersion7(),
            DateTime.UtcNow,
            null,
            null,
            null
        );
}
