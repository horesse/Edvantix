using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Create;

internal sealed class CreatePaymentMethodValidator
    : OrganizationScopedLookupValidator<CreatePaymentMethodCommand>
{
    public CreatePaymentMethodValidator(
        PaymentMethodUniqueNameChecker nameChecker,
        IPaymentMethodRepository repository,
        ITenantContext tenantContext
    )
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name)
    {
        RuleFor(c => c.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Код способа оплаты не может быть пустым.")
            .MaximumLength(PaymentMethod.MaxCodeLength)
            .WithMessage(
                $"Код способа оплаты не может превышать {PaymentMethod.MaxCodeLength} символов."
            )
            .MustAsync(
                async (code, ct) =>
                    !await repository.AnyAsync(
                        new PaymentMethodUniqueCodeSpecification(
                            tenantContext.OrganizationId,
                            code.Trim()
                        ),
                        ct
                    )
            )
            .WithMessage("Способ оплаты с таким кодом уже существует в этой организации.");

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок не может быть отрицательным.");
    }
}
