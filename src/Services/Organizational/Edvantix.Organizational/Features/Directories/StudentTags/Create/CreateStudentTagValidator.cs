using System.Text.RegularExpressions;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentTags.Create;

internal sealed class CreateStudentTagValidator
    : OrganizationScopedLookupValidator<CreateStudentTagCommand>
{
    private static readonly Regex HexColorRegex = new(
        @"^#[0-9A-Fa-f]{6}$",
        RegexOptions.Compiled,
        TimeSpan.FromMilliseconds(100)
    );

    public CreateStudentTagValidator(
        StudentTagUniqueNameChecker nameChecker,
        ITenantContext tenantContext
    )
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name)
    {
        RuleFor(c => c.Name)
            .MaximumLength(Domain.AggregatesModel.StudentTagAggregate.StudentTag.MaxNameLength)
            .WithMessage(
                $"Название тега не может превышать {Domain.AggregatesModel.StudentTagAggregate.StudentTag.MaxNameLength} символов."
            );

        RuleFor(c => c.Color)
            .NotEmpty()
            .WithMessage("Цвет тега обязателен.")
            .Matches(HexColorRegex)
            .WithMessage("Цвет тега должен быть в формате HEX #RRGGBB (например, #FF5733).");

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок не может быть отрицательным.");
    }
}
