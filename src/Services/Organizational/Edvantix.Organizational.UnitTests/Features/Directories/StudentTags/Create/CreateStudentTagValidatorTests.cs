using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.Create;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Create;

public sealed class CreateStudentTagValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        CreateStudentTagValidator validator,
        Mock<IStudentTagRepository> repoMock
    ) CreateValidator(bool nameExists = false)
    {
        var tenantMock = new Mock<ITenantContext>();
        tenantMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var repoMock = new Mock<IStudentTagRepository>();
        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<StudentTag>>(s => s is StudentTagUniqueNameSpecification),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(nameExists);

        var nameChecker = new StudentTagUniqueNameChecker(repoMock.Object);
        var validator = new CreateStudentTagValidator(nameChecker, tenantMock.Object);

        return (validator, repoMock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand("VIP", "#FF5733");

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
        var command = new CreateStudentTagCommand(name!, "#FF5733");

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateStudentTagCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNameLongerThan40Chars_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand(
            new string('А', StudentTag.MaxNameLength + 1),
            "#FF5733"
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Test]
    public async Task GivenNameExactly40Chars_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand(
            new string('А', StudentTag.MaxNameLength),
            "#FF5733"
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Name);
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new CreateStudentTagCommand("VIP", "#FF5733");

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateStudentTagCommand>.NameProperty
        );
    }

    [Test]
    [Arguments("")]
    [Arguments("   ")]
    [Arguments("FF5733")]
    [Arguments("#GG5733")]
    [Arguments("#FF573")]
    [Arguments("#FF57330")]
    public async Task GivenInvalidColor_WhenValidating_ThenShouldFail(string color)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand("VIP", color);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Color);
    }

    [Test]
    [Arguments("#000000")]
    [Arguments("#FFFFFF")]
    [Arguments("#ff5733")]
    [Arguments("#aAbBcC")]
    public async Task GivenValidColor_WhenValidating_ThenShouldBeValid(string color)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand("VIP", color);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Color);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand("VIP", "#FF5733", Order: -1);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }

    [Test]
    public async Task GivenZeroOrder_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentTagCommand("VIP", "#FF5733", Order: 0);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Order);
    }
}
