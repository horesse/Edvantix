using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Archive;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Archive;

public sealed class ArchivePaymentMethodCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<ClaimsPrincipal> _claimsMock = new();
    private readonly Mock<IPaymentMethodRepository> _repoMock = new();
    private readonly Guid _orgId = Guid.CreateVersion7();
    private readonly Guid _profileId = Guid.CreateVersion7();
    private readonly ArchivePaymentMethodCommandHandler _handler;

    public ArchivePaymentMethodCommandHandlerTests()
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
    public async Task GivenActivePaymentMethod_WhenArchiving_ThenShouldSetIsArchivedAndSave()
    {
        var pm = CreatePaymentMethod(_orgId);
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);

        await _handler.Handle(new ArchivePaymentMethodCommand(pm.Id), CancellationToken.None);

        pm.IsDeleted.ShouldBeTrue();
        _repoMock.Verify(
            r => r.UnitOfWork.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAlreadyArchivedPaymentMethod_WhenArchiving_ThenShouldBeIdempotent()
    {
        var pm = CreatePaymentMethod(_orgId);
        pm.Archive(_profileId);
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);

        await _handler.Handle(new ArchivePaymentMethodCommand(pm.Id), CancellationToken.None);

        pm.IsDeleted.ShouldBeTrue();
    }

    [Test]
    public async Task GivenPaymentMethodNotFound_WhenArchiving_ThenShouldThrowNotFoundException()
    {
        var id = Guid.CreateVersion7();
        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PaymentMethod?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchivePaymentMethodCommand(id), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenPaymentMethodFromDifferentOrganization_WhenArchiving_ThenShouldThrowNotFoundException()
    {
        var pm = CreatePaymentMethod(Guid.CreateVersion7());
        _repoMock.Setup(r => r.GetByIdAsync(pm.Id, It.IsAny<CancellationToken>())).ReturnsAsync(pm);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new ArchivePaymentMethodCommand(pm.Id), CancellationToken.None).AsTask()
        );
    }

    private static PaymentMethod CreatePaymentMethod(Guid orgId) =>
        new(orgId, "Карта", "card", true, false);
}
