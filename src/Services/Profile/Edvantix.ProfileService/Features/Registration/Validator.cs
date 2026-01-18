using Edvantix.ProfileService.Features.EmploymentHistoryFeature.Models;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Features.UserContactFeature.Models;
using FluentValidation;

namespace Edvantix.ProfileService.Features.Registration;

public sealed class Validator : AbstractValidator<RegistrationCommand>
{
    public Validator(
        IValidator<UserContactModel> validator,
        IValidator<EmploymentHistoryModel> ehValidator,
        IValidator<ProfileModel> piValidator
    )
    {
        RuleFor(x => x.Contacts).ForEach(c => c.SetValidator(validator));

        RuleFor(x => x.Gender).NotEmpty().WithMessage("Пол не может быть пустым");

        RuleFor(x => x.EmploymentHistories).ForEach(eh => eh.SetValidator(ehValidator));

        RuleFor(x => x.Profile).SetValidator(piValidator);
    }
}
