using Edvantix.Person.Features.ContactFeature.Models;
using Edvantix.Person.Features.EmploymentHistoryFeature.Models;
using Edvantix.Person.Features.PersonInfoFeature.Models;
using FluentValidation;

namespace Edvantix.Person.Features.Registration;

public sealed class RegistrationValidator : AbstractValidator<RegistrationCommand>
{
    public RegistrationValidator(
        IValidator<ContactModel> validator,
        IValidator<EmploymentHistoryModel> ehValidator,
        IValidator<PersonInfoModel> piValidator
    )
    {
        RuleFor(x => x.Contacts).ForEach(c => c.SetValidator(validator));

        RuleFor(x => x.Gender).NotEmpty().WithMessage("Пол не может быть пустым");

        RuleFor(x => x.EmploymentHistories).ForEach(eh => eh.SetValidator(ehValidator));

        RuleFor(x => x.PersonInfo).SetValidator(piValidator);
    }
}
