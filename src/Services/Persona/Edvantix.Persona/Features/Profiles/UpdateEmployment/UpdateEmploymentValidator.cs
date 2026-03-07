namespace Edvantix.Persona.Features.Profiles.UpdateEmployment;

public sealed class UpdateEmploymentValidator : AbstractValidator<UpdateEmploymentCommand>
{
    public UpdateEmploymentValidator()
    {
        RuleForEach(x => x.EmploymentHistories)
            .SetValidator(new EmploymentHistoryRequestValidator());
    }
}
