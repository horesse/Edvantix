namespace Edvantix.Persona.Features.Profiles.UpdateEducation;

public sealed class UpdateEducationValidator : AbstractValidator<UpdateEducationCommand>
{
    public UpdateEducationValidator()
    {
        RuleForEach(x => x.Educations).SetValidator(new EducationRequestValidator());
    }
}
