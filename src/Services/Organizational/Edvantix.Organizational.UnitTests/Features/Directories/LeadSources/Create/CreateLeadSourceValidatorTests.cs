namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Create;

public sealed class CreateLeadSourceValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        CreateLeadSourceValidator validator,
        Mock<ILeadSourceRepository> repoMock
    ) CreateValidator(bool nameExists = false)
    {
        var tenantMock = new Mock<ITenantContext>();
        tenantMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var repoMock = new Mock<ILeadSourceRepository>();
        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<LeadSource>>(s => s is LeadSourceUniqueNameSpecification),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(nameExists);

        var nameChecker = new LeadSourceUniqueNameChecker(repoMock.Object);
        var validator = new CreateLeadSourceValidator(nameChecker, tenantMock.Object);

        return (validator, repoMock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, "utm_insta");

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyName_WhenValidating_ThenShouldFailOnName(string? name)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateLeadSourceCommand(name!, LeadChannel.Online, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateLeadSourceCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNameLongerThan120_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateLeadSourceCommand(new string('А', 121), LeadChannel.Online, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateLeadSourceCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateLeadSourceCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenUtmTagExceedingMaxLength_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var longTag = new string('x', LeadSource.MaxUtmTagLength + 1);
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, longTag);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.UtmTag);
    }

    [Test]
    public async Task GivenNullUtmTag_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateLeadSourceCommand("Флаер", LeadChannel.Offline, null);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UtmTag);
    }

    [Test]
    public async Task GivenUtmTagAtMaxLength_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var tag = new string('x', LeadSource.MaxUtmTagLength);
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, tag);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.UtmTag);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, null, Order: -1);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }

    [Test]
    public async Task GivenZeroOrder_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateLeadSourceCommand("Инстаграм", LeadChannel.Online, null, Order: 0);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Order);
    }
}
