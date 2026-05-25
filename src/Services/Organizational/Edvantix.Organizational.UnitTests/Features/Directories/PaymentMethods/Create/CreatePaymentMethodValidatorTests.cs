using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Create;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Create;

public sealed class CreatePaymentMethodValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        CreatePaymentMethodValidator validator,
        Mock<IPaymentMethodRepository> repoMock
    ) CreateValidator(bool nameExists = false, bool codeExists = false)
    {
        var tenantMock = new Mock<ITenantContext>();
        tenantMock.Setup(t => t.OrganizationId).Returns(OrgId);

        var repoMock = new Mock<IPaymentMethodRepository>();
        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<PaymentMethod>>(s =>
                        s is PaymentMethodUniqueNameSpecification
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(nameExists);

        repoMock
            .Setup(r =>
                r.AnyAsync(
                    It.Is<ISpecification<PaymentMethod>>(s =>
                        s is PaymentMethodUniqueCodeSpecification
                    ),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(codeExists);

        var nameChecker = new PaymentMethodUniqueNameChecker(repoMock.Object);
        var validator = new CreatePaymentMethodValidator(
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
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false);

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
        var command = new CreatePaymentMethodCommand(name!, "card", true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreatePaymentMethodCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenNameLongerThan120_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreatePaymentMethodCommand(new string('А', 121), "card", true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreatePaymentMethodCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<CreatePaymentMethodCommand>.NameProperty
        );
    }

    [Test]
    [Arguments(null)]
    [Arguments("")]
    [Arguments("   ")]
    public async Task GivenEmptyCode_WhenValidating_ThenShouldFailOnCode(string? code)
    {
        var (validator, _) = CreateValidator();
        var command = new CreatePaymentMethodCommand("Карта", code!, true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenCodeLongerThan20_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var longCode = new string('x', PaymentMethod.MaxCodeLength + 1);
        var command = new CreatePaymentMethodCommand("Карта", longCode, true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenCodeAtMaxLength_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var maxCode = new string('x', PaymentMethod.MaxCodeLength);
        var command = new CreatePaymentMethodCommand("Карта", maxCode, true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenDuplicateCode_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(codeExists: true);
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false, Order: -1);

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }

    [Test]
    public async Task GivenZeroOrder_WhenValidating_ThenShouldBeValid()
    {
        var (validator, _) = CreateValidator();
        var command = new CreatePaymentMethodCommand("Карта", "card", true, false, Order: 0);

        var result = await validator.TestValidateAsync(command);

        result.ShouldNotHaveValidationErrorFor(c => c.Order);
    }
}
