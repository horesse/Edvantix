using FluentValidation;

namespace Edvantix.ProfileService.Features.EducationFeature.Features.UpdateOwnEducations;

/// <summary>
/// Валидатор команды обновления образования
/// </summary>
public sealed class UpdateOwnEducationsValidator : AbstractValidator<UpdateOwnEducationsCommand>
{
    public UpdateOwnEducationsValidator()
    {
        RuleForEach(x => x.Educations).SetValidator(new Validator());
    }
}
