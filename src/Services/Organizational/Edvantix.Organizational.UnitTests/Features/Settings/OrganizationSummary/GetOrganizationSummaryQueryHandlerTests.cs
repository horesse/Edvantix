using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Features.Organizations;
using Edvantix.Organizational.Features.Settings.OrganizationSummary;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Organizational.UnitTests.Features.Settings.OrganizationSummary;

public sealed class GetOrganizationSummaryQueryHandlerTests : IDisposable
{
    private readonly Mock<ITenantContext> _tenantMock = new();
    private readonly Mock<IOrganizationRepository> _repoMock = new();
    private readonly Mock<IOrganizationMemberRepository> _memberRepoMock = new();
    private readonly Mock<IProfileService> _profileServiceMock = new();
    private readonly IFusionCache _cache = new FusionCache(new FusionCacheOptions());
    private readonly GetOrganizationSummaryQueryHandler _handler;

    private readonly Guid _orgId = Guid.CreateVersion7();

    private static readonly Guid ValidCountryId = Guid.CreateVersion7();
    private static readonly Guid ValidCurrencyId = Guid.CreateVersion7();

    public GetOrganizationSummaryQueryHandlerTests()
    {
        _tenantMock.Setup(t => t.OrganizationId).Returns(_orgId);

        _handler = new(
            _tenantMock.Object,
            _repoMock.Object,
            _memberRepoMock.Object,
            _profileServiceMock.Object,
            _cache
        );
    }

    [Test]
    public async Task GivenOrganizationWithPrimaryContact_WhenQuerying_ThenShouldReturnPrimaryContact()
    {
        var org = CreateOrganization();
        var contact = new Contact(org.Id, "info@test.ru", "Основной", ContactType.Email, isPrimary: true);
        org.AddContact(contact);

        SetupRepo(org);
        SetupMembersCount(5);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.PrimaryContact.ShouldNotBeNull();
        result.PrimaryContact!.Value.ShouldBe("info@test.ru");
        result.PrimaryContact.IsPrimary.ShouldBeTrue();
    }

    [Test]
    public async Task GivenOrganizationWithMultipleContacts_WhenQuerying_ThenShouldReturnOnlyPrimary()
    {
        var org = CreateOrganization();
        org.AddContact(new Contact(org.Id, "other@test.ru", "Вторичный", ContactType.Email, isPrimary: false));
        org.AddContact(new Contact(org.Id, "+79001234567", "Основной телефон", ContactType.MobilePhone, isPrimary: true));

        SetupRepo(org);
        SetupMembersCount(0);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.PrimaryContact.ShouldNotBeNull();
        result.PrimaryContact!.ContactType.ShouldBe(ContactType.MobilePhone);
    }

    [Test]
    public async Task GivenOrganizationWithLastModifiedByNull_WhenQuerying_ThenLastModifiedByDisplayNameShouldBeNull()
    {
        var org = CreateOrganization();
        SetupRepo(org);
        SetupMembersCount(0);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.LastModified.ByDisplayName.ShouldBeNull();
        _profileServiceMock.Verify(
            p => p.GetProfileByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }

    [Test]
    public async Task GivenOrganizationWithKnownModifier_WhenQuerying_ThenShouldReturnModifierDisplayName()
    {
        var profileId = Guid.CreateVersion7();
        var org = CreateOrganizationWithLastModifiedBy(profileId);
        SetupRepo(org);
        SetupMembersCount(3);

        var profileResponse = new GetProfileResponse { FullName = "Иван Иванов" };
        _profileServiceMock
            .Setup(p => p.GetProfileByIdAsync(profileId.ToString("D"), It.IsAny<CancellationToken>()))
            .ReturnsAsync(profileResponse);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.LastModified.ByDisplayName.ShouldBe("Иван Иванов");
    }

    [Test]
    public async Task GivenOrganizationWithDeletedModifier_WhenQuerying_ThenShouldReturnFallbackDisplayName()
    {
        var profileId = Guid.CreateVersion7();
        var org = CreateOrganizationWithLastModifiedBy(profileId);
        SetupRepo(org);
        SetupMembersCount(0);

        _profileServiceMock
            .Setup(p => p.GetProfileByIdAsync(profileId.ToString("D"), It.IsAny<CancellationToken>()))
            .ReturnsAsync((GetProfileResponse?)null);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.LastModified.ByDisplayName.ShouldBe("Удалённый пользователь");
    }

    [Test]
    public async Task GivenOrganizationWithNoMembers_WhenQuerying_ThenMembersCountShouldBeZero()
    {
        var org = CreateOrganization();
        SetupRepo(org);
        SetupMembersCount(0);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.MembersCount.ShouldBe(0);
    }

    [Test]
    public async Task GivenOrganizationWithMembers_WhenQuerying_ThenMembersCountShouldBeCorrect()
    {
        var org = CreateOrganization();
        SetupRepo(org);
        SetupMembersCount(42);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.MembersCount.ShouldBe(42);
    }

    [Test]
    public async Task GivenOrganizationAuditFields_WhenQuerying_ThenShouldReturnCorrectAuditData()
    {
        var org = CreateOrganization();
        SetupRepo(org);
        SetupMembersCount(1);

        var result = await _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None);

        result.Id.ShouldBe(_orgId);
        result.FullLegalName.ShouldBe("ООО Тестовая Компания");
        result.OrganizationType.ShouldBe(OrganizationType.PrivateEducationalCenter);
        result.Status.ShouldBe(OrganizationStatus.Active);
        result.IsLegalEntity.ShouldBeTrue();
    }

    [Test]
    public async Task GivenOrganizationNotFound_WhenQuerying_ThenShouldThrowNotFoundException()
    {
        _repoMock
            .Setup(r => r.GetByIdAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Organization?)null);

        await Should.ThrowAsync<NotFoundException>(() =>
            _handler.Handle(new GetOrganizationSummaryQuery(), CancellationToken.None).AsTask()
        );
    }

    public void Dispose() => _cache.Dispose();

    private void SetupRepo(Organization org) =>
        _repoMock
            .Setup(r => r.GetByIdAsync(_orgId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(org);

    private void SetupMembersCount(int count) =>
        _memberRepoMock
            .Setup(r => r.CountAsync(It.IsAny<ISpecification<OrganizationMember>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(count);

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
            Id = _orgId,
        };

    private Organization CreateOrganizationWithLastModifiedBy(Guid profileId)
    {
        var org = CreateOrganization();
        org.Update(
            org.FullLegalName,
            org.ShortName,
            org.OrganizationType,
            org.LegalForm,
            org.RegistrationDate,
            profileId
        );
        return org;
    }
}
