using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Features.Directories.PaymentMethods;
using Edvantix.Organizational.Features.Directories.PaymentMethods.Update;

namespace Edvantix.Organizational.UnitTests.Features.Directories.PaymentMethods.Update;

public sealed class UpdatePaymentMethodValidatorTests
{
    private static readonly Guid OrgId = Guid.CreateVersion7();

    private static (
        UpdatePaymentMethodValidator validator,
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
        var validator = new UpdatePaymentMethodValidator(
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
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            "Карта",
            "card",
            true,
            false
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
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            name!,
            "card",
            true,
            false
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<UpdatePaymentMethodCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenDuplicateName_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(nameExists: true);
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            "Карта",
            "card",
            true,
            false
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(
            OrganizationScopedLookupValidator<UpdatePaymentMethodCommand>.NameProperty
        );
    }

    [Test]
    public async Task GivenDuplicateCode_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator(codeExists: true);
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            "Карта",
            "card",
            true,
            false
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenCodeLongerThan20_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var longCode = new string('x', PaymentMethod.MaxCodeLength + 1);
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            "Карта",
            longCode,
            true,
            false
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Code);
    }

    [Test]
    public async Task GivenNegativeOrder_WhenValidating_ThenShouldFail()
    {
        var (validator, _) = CreateValidator();
        var command = new UpdatePaymentMethodCommand(
            Guid.CreateVersion7(),
            "Карта",
            "card",
            true,
            false,
            Order: -1
        );

        var result = await validator.TestValidateAsync(command);

        result.ShouldHaveValidationErrorFor(c => c.Order);
    }
}
