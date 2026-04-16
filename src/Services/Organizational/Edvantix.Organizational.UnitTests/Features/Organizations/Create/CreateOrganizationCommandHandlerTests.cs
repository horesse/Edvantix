using Edvantix.Chassis.Security.Keycloak;

namespace Edvantix.Organizational.UnitTests.Features.Organizations.Create;

public sealed class CreateOrganizationCommandHandlerTests
{
    private readonly Mock<IOrganizationRepository> _repoMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Guid _profileId = Guid.CreateVersion7();

    public CreateOrganizationCommandHandlerTests()
    {
        _repoMock.Setup(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
        _unitOfWorkMock
            .Setup(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    [Test]
    public async Task GivenAuthenticatedUser_WhenHandling_ThenShouldReturnOrganizationId()
    {
        var handler = CreateHandler(_profileId);
        var command = BuildValidCommand();

        var id = await handler.Handle(command, CancellationToken.None);

        id.ShouldNotBe(Guid.Empty);
    }

    [Test]
    public async Task GivenAuthenticatedUser_WhenHandling_ThenShouldAddOrganizationToRepository()
    {
        var handler = CreateHandler(_profileId);
        var command = BuildValidCommand();

        await handler.Handle(command, CancellationToken.None);

        _repoMock.Verify(
            r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public async Task GivenAuthenticatedUser_WhenHandling_ThenShouldSaveEntities()
    {
        var handler = CreateHandler(_profileId);
        var command = BuildValidCommand();

        await handler.Handle(command, CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.SaveEntitiesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task GivenAuthenticatedUser_WhenHandling_ThenOrganizationShouldHavePrimaryContact()
    {
        Organization? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
            .Callback<Organization, CancellationToken>((org, _) => captured = org);

        var handler = CreateHandler(_profileId);
        var command = BuildValidCommand();

        await handler.Handle(command, CancellationToken.None);

        captured.ShouldNotBeNull();
        captured.Contacts.ShouldHaveSingleItem();
        captured.Contacts[0].IsPrimary.ShouldBeTrue();
        captured.Contacts[0].Value.ShouldBe(command.PrimaryContactValue);
        captured.Contacts[0].ContactType.ShouldBe(command.PrimaryContactType);
    }

    [Test]
    public async Task GivenAuthenticatedUser_WhenHandling_ThenOrganizationShouldHaveDomainEvent()
    {
        Organization? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
            .Callback<Organization, CancellationToken>((org, _) => captured = org);

        var handler = CreateHandler(_profileId);

        await handler.Handle(BuildValidCommand(), CancellationToken.None);

        captured.ShouldNotBeNull();
        var @event = captured
            .DomainEvents.Single()
            .ShouldBeOfType<OrganizationCreatedDomainEvent>();
        @event.OwnerProfileId.ShouldBe(_profileId);
    }

    [Test]
    public async Task GivenAuthenticatedUser_WhenHandling_ThenOrganizationShouldHaveCorrectProperties()
    {
        Organization? captured = null;
        _repoMock
            .Setup(r => r.AddAsync(It.IsAny<Organization>(), It.IsAny<CancellationToken>()))
            .Callback<Organization, CancellationToken>((org, _) => captured = org);

        var handler = CreateHandler(_profileId);
        var command = BuildValidCommand();

        await handler.Handle(command, CancellationToken.None);

        captured.ShouldNotBeNull();
        captured.FullLegalName.ShouldBe(command.FullLegalName);
        captured.IsLegalEntity.ShouldBe(command.IsLegalEntity);
        captured.LegalForm.ShouldBe(command.LegalForm);
        captured.OrganizationType.ShouldBe(command.OrganizationType);
        captured.Status.ShouldBe(OrganizationStatus.Active);
    }

    [Test]
    public async Task GivenUserWithoutProfileId_WhenHandling_ThenShouldThrowException()
    {
        var claims = new ClaimsPrincipal(new ClaimsIdentity());
        var handler = new CreateOrganizationCommandHandler(claims, _repoMock.Object);

        await Should.ThrowAsync<Exception>(() =>
            handler.Handle(BuildValidCommand(), CancellationToken.None).AsTask()
        );
    }

    private CreateOrganizationCommandHandler CreateHandler(Guid profileId)
    {
        var claims = new ClaimsPrincipal(
            new ClaimsIdentity([new Claim(KeycloakClaimTypes.Profile, profileId.ToString())])
        );
        return new(claims, _repoMock.Object);
    }

    private static CreateOrganizationCommand BuildValidCommand() =>
        new(
            "ООО Тестовая Организация",
            "ТестОрг",
            IsLegalEntity: true,
            new DateOnly(2020, 1, 1),
            LegalForm.Llc,
            Guid.CreateVersion7(),
            Guid.CreateVersion7(),
            OrganizationType.PrivateEducationalCenter,
            "info@test.com",
            ContactType.Email,
            "Основной контакт"
        );
}
