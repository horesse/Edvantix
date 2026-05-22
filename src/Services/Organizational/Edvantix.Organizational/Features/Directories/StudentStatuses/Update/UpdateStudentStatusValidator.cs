using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses.Update;

internal sealed class UpdateStudentStatusValidator
    : OrganizationScopedLookupValidator<UpdateStudentStatusCommand>
{
    public UpdateStudentStatusValidator(
        StudentStatusUniqueNameChecker nameChecker,
        IStudentStatusRepository repository,
        ITenantContext tenantContext
    )
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name, c => c.Id)
    {
        RuleFor(c => c.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Код статуса не может быть пустым.")
            .MaximumLength(StudentStatus.MaxCodeLength)
            .WithMessage($"Код статуса не может превышать {StudentStatus.MaxCodeLength} символов.")
            .MustAsync(
                async (cmd, code, ct) =>
                    !await repository.ExistsCodeAsync(
                        tenantContext.OrganizationId,
                        code.Trim(),
                        excludeId: cmd.Id,
                        ct
                    )
            )
            .WithMessage("Статус с таким кодом уже существует в этой организации.");

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок не может быть отрицательным.");
    }
}
