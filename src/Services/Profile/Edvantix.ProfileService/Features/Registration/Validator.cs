using Edvantix.ProfileService.Features.ProfileFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.Registration;

public sealed class Validator : AbstractValidator<RegistrationCommand>
{
    public Validator(
        IValidator<ProfileModel> piValidator
    )
    {
        RuleFor(x => x.Gender).NotEmpty().WithMessage("Пол не может быть пустым");
        
        RuleFor(x => x.Profile).SetValidator(piValidator);
    }
}
