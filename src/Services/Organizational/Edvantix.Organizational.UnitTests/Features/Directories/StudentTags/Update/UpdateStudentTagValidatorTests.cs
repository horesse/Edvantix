using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;
using Edvantix.Organizational.Features.Directories.StudentTags;
using Edvantix.Organizational.Features.Directories.StudentTags.Update;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentTags.Update;

public sealed class UpdateStudentTagValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        UpdateStudentTagValidator validator,
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
        var validator = new UpdateStudentTagValidator(nameChecker, tenantMock.Object);

        return (validator, repoMock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new UpdateStudentTagCommand(Guid.CreateVersion7(), "VIP", "#FF5733");

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
        var command = new UpdateStudentTagCommand(Guid.CreateVersion7(), name!, "#FF5733");

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<UpdateStudentTagCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNameLongerThan40Chars_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new UpdateStudentTagCommand(
            Guid.CreateVersion7(),
            new string('А', StudentTag.MaxNameLength + 1),
            "#FF5733"
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Name);
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new UpdateStudentTagCommand(Guid.CreateVersion7(), "VIP", "#FF5733");

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<UpdateStudentTagCommand>.NameProperty
        );
    }

    [Test]
    [Arguments("")]
    [Arguments("FF5733")]
    [Arguments("#GG5733")]
    [Arguments("#FF573")]
    public async Task GivenInvalidColor_WhenValidating_ThenShouldFail(string color)
    {
        var (validator, _) = CreateValidator();
        var command = new UpdateStudentTagCommand(Guid.CreateVersion7(), "VIP", color);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Color);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new UpdateStudentTagCommand(
            Guid.CreateVersion7(),
            "VIP",
            "#FF5733",
            Order: -1
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }
}
