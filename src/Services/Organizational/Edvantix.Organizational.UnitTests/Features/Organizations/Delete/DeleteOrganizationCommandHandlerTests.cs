namespace Edvantix.Organizational.UnitTests.Features.Organizations.Delete;

public sealed class DeleteOrganizationCommandHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ITenantContext> _tenantContextMock = new();
    private readonly DeleteOrganizationCommandHandler _handler;

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    public DeleteOrganizationCommandHandlerTests()
    {
        _repoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _handler = new(_repoMock.Object, _tenantContextMock.Object);
    }

    [Test]
    public async Task GivenExistingOrganization_WhenHandling_ThenShouldMarkAsDeleted()
    {
        var orgId = Guid.CreateVersion7();
        var org = CreateOrganization(orgId);
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(org);

        await _handler.Handle(new DeleteOrganizationCommand(orgId), CancellationToken.None);

        org.IsDeleted.ShouldBeTrue();
        org.Status.ShouldBe(OrganizationStatus.Deleted);
    }

    [Test]
    public async Task GivenExistingOrganization_WhenHandling_ThenShouldSaveEntities()
    {
        var orgId = Guid.CreateVersion7();
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateOrganization(orgId));

        await _handler.Handle(new DeleteOrganizationCommand(orgId), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenOrganizationNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        var orgId = Guid.CreateVersion7();
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new DeleteOrganizationCommand(orgId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenOrganizationNotFound_WhenHandling_ThenShouldNotSave()
    {
        var orgId = Guid.CreateVersion7();
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(orgId);
        _repoMock
            .Setup(r => r.GetByIdAsync(orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization?)null);

        try
        {
            await _handler.Handle(new DeleteOrganizationCommand(orgId), CancellationToken.None);
        }
        catch (NotFoundException) { }

        _unitOfWorkMock.Verify(
            u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenDifferentTenantId_WhenHandling_ThenShouldThrowForbiddenException()
    {
        var orgId = Guid.CreateVersion7();
        var differentId = Guid.CreateVersion7();
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(differentId);

        await Should.ThrowAsync<ForbiddenException>(() =>
            _handler.Handle(new DeleteOrganizationCommand(orgId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenDifferentTenantId_WhenHandling_ThenShouldNotCallRepository()
    {
        var orgId = Guid.CreateVersion7();
        _tenantContextMock.Setup(t => t.OrganizationId).Returns(Guid.CreateVersion7());

        try
        {
            await _handler.Handle(new DeleteOrganizationCommand(orgId), CancellationToken.None);
        }
        catch (ForbiddenException) { }

        _repoMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never
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
}
