namespace Edvantix.Organizational.UnitTests.Features.Directories.LeadSources.Update;

public sealed class UpdateLeadSourceValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        UpdateLeadSourceValidator validator,
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
        var validator = new UpdateLeadSourceValidator(nameChecker, tenantMock.Object);

        return (validator, repoMock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new UpdateLeadSourceCommand(
            Guid.CreateVersion7(),
            "Инстаграм",
            LeadChannel.Online,
            "utm_insta"
        );

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
        var command = new UpdateLeadSourceCommand(
            Guid.CreateVersion7(),
            name!,
            LeadChannel.Online,
            null
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<UpdateLeadSourceCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenUtmTagExceedingMaxLength_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var longTag = new string('x', LeadSource.MaxUtmTagLength + 1);
        var command = new UpdateLeadSourceCommand(
            Guid.CreateVersion7(),
            "Инстаграм",
            LeadChannel.Online,
            longTag
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.UtmTag);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new UpdateLeadSourceCommand(
            Guid.CreateVersion7(),
            "Инстаграм",
            LeadChannel.Online,
            null,
            -1
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }
}
