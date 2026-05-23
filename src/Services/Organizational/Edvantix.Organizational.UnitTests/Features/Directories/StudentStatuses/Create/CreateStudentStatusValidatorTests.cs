using Edvantix.Chassis.Specification;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Directories.StudentStatuses;
using Edvantix.Organizational.Features.Directories.StudentStatuses.Create;
using Edvantix.Organizational.Features.Directories.StudentStatuses.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.UnitTests.Features.Directories.StudentStatuses.Create;

public sealed class CreateStudentStatusValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        CreateStudentStatusValidator validator,
        Mock<IStudentStatusRepository> repoMock
    ) CreateValidator(bool nameExists = false, bool codeExists = false)
    {
        var tenantMock = new Mock<ITenantContext>();
        tenantMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var repoMock = new Mock<IStudentStatusRepository>();

        // Уникальность имени проверяется через StudentStatusUniqueNameSpecification
        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<StudentStatus>>(s =>
                        s is StudentStatusUniqueNameSpecification
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(nameExists);

        // Уникальность кода проверяется через StudentStatusUniqueCodeSpecification
        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<StudentStatus>>(s =>
                        s is StudentStatusUniqueCodeSpecification
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(codeExists);

        var nameChecker = new StudentStatusUniqueNameChecker(repoMock.Object);
        var validator = new CreateStudentStatusValidator(
            nameChecker,
            repoMock.Object,
            tenantMock.Object
        );

        return (validator, repoMock);
    }

    [Test]
    public async Task GivenValidCommand_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentStatusCommand(
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active
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
        var command = new CreateStudentStatusCommand(name!, "ACTIVE", StudentStatusTone.Active);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateStudentStatusCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNameLongerThan120_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentStatusCommand(
            new string('A', 121),
            "ACTIVE",
            StudentStatusTone.Active
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateStudentStatusCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new CreateStudentStatusCommand(
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreateStudentStatusCommand>.NameProperty
        );
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyCode_WhenValidating_ThenShouldFailOnCode(string? code)
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentStatusCommand("Активный", code!, StudentStatusTone.Active);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenCodeLongerThan20_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var code = new string('X', StudentStatus.MaxCodeLength + 1);
        var command = new CreateStudentStatusCommand("Активный", code, StudentStatusTone.Active);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenDuplicateCode_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(codeExists: true);
        var command = new CreateStudentStatusCommand(
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentStatusCommand(
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            Order: -1
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }

    [Test]
    public async Task GivenZeroOrder_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreateStudentStatusCommand(
            "Активный",
            "ACTIVE",
            StudentStatusTone.Active,
            Order: 0
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Order);
    }
}
