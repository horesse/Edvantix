using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Create;

internal sealed class CreateStudentStatusValidator
    : OrganizationScopedLookupValidator<CreateStudentStatusCommand>
{
    public CreateStudentStatusValidator(
        StudentStatusUniqueNameChecker nameChecker,
        IStudentStatusRepository repository,
        ITenantContext tenantContext
    )
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name)
    {
        RuleFor(c => c.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Код статуса не может быть пустым.")
            .MaximumLength(StudentStatus.MaxCodeLength)
            .WithMessage($"Код статуса не может превышать {StudentStatus.MaxCodeLength} символов.")
            .MustAsync(
                async (code, ct) =>
                    !await repository.AnyAsync(
                        new StudentStatusUniqueCodeSpecification(
                            tenantContext.OrganizationId,
                            code.Trim()
                        ),
                        ct
                    )
            )
            .WithMessage("Статус с таким кодом уже существует в этой организации.");

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок не может быть отрицательным.");
    }
}
