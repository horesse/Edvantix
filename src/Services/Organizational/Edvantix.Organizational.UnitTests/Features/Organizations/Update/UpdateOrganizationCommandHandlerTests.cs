using Edvantix.Chassis.Security.Tenant;

namespace Edvantix.Organizational.UnitTests.Features.Organizations.Update;

public sealed class UpdateOrganizationCommandHandlerTests
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly UpdateOrganizationCommandHandler _handler;
    private readonly Guid _organizationId = Guid.CreateVersion7();

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    public UpdateOrganizationCommandHandlerTests()
    {
        _repoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _tenantMock.Setup(t => t.OrganizationId).Returns(_organizationId);

        _handler = new(_repoMock.Object, _tenantMock.Object);
    }

    [Test]
    public async Task GivenExistingOrganization_WhenHandling_ThenShouldUpdateProperties()
    {
        var org = CreateOrganization();
        _repoMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(org);

        var command = new UpdateOrganizationCommand(
            _organizationId,
            "АО Новое Название",
            "НовНаз",
            OrganizationType.University,
            LegalForm.Ojsc
        );

        await _handler.Handle(command, CancellationToken.None);

        org.FullLegalName.ShouldBe("АО Новое Название");
        org.ShortName.ShouldBe("НовНаз");
        org.OrganizationType.ShouldBe(OrganizationType.University);
        org.LegalForm.ShouldBe(LegalForm.Ojsc);
    }

    [Test]
    public async Task GivenExistingOrganization_WhenHandling_ThenShouldSaveEntities()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateOrganization());

        await _handler.Handle(BuildValidCommand(_organizationId), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenOrganizationNotFound_WhenHandling_ThenShouldThrowNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(BuildValidCommand(_organizationId), CancellationToken.None).AsTask()
        );
    }

    [Test]
    public async Task GivenExistingOrganization_WhenHandlingWithNullShortName_ThenShouldClearShortName()
    {
        var org = CreateOrganization();
        _repoMock
            .Setup(r => r.GetByIdAsync(_organizationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(org);

        var command = BuildValidCommand(_organizationId) with { ShortName = null };

        await _handler.Handle(command, CancellationToken.None);

        org.ShortName.ShouldBeNull();
    }

    private Organization CreateOrganization() =>
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
            Id = _organizationId,
        };

    private static UpdateOrganizationCommand BuildValidCommand(Guid id) =>
        new(id, "ООО Обновлённая Компания", "ОбнКо", OrganizationType.ItSchool, LegalForm.Pue);
}
