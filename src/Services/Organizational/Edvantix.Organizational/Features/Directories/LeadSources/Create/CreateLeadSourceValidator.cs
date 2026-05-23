using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.LeadSources.Create;

internal sealed class CreateLeadSourceValidator
    : OrganizationScopedLookupValidator<CreateLeadSourceCommand>
{
    public CreateLeadSourceValidator(
        LeadSourceUniqueNameChecker nameChecker,
        ITenantContext tenantContext
    )
        : base(nameChecker, _ => tenantContext.OrganizationId, c => c.Name)
    {
        RuleFor(c => c.UtmTag)
            .MaximumLength(LeadSource.MaxUtmTagLength)
            .WithMessage($"UTM-метка не может превышать {LeadSource.MaxUtmTagLength} символов.")
            .When(c => c.UtmTag is not null);

        RuleFor(c => c.Order)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Порядок не может быть отрицательным.");
    }
}
