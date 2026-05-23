using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.PaymentMethodAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.PaymentMethods.Update;

internal sealed class UpdatePaymentMethodValidator
    : OrganizationScopedLookupValidator<UpdatePaymentMethodCommand>
{
    public UpdatePaymentMethodValidator(
        PaymentMethodUniqueNameChecker nameChecker,
        IPaymentMethodRepository repository,
        ITenantContext tenantContext
    )
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name, c => c.Id)
    {
        RuleFor(c => c.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Код способа оплаты не может быть пустым.")
            .MaximumLength(PaymentMethod.MaxCodeLength)
            .WithMessage($"Код способа оплаты не может превышать {PaymentMethod.MaxCodeLength} символов.")
            .MustAsync(
                async (cmd, code, ct) =>
                    !await repository.AnyAsync(
                        new PaymentMethodUniqueCodeSpecification(
                            tenantContext.OrganizationId,
                            code.Trim(),
                            excludeId: cmd.Id
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
