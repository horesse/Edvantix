using Edvantix.Person.Features.PersonInfo.Models;
using FluentValidation;

namespace Edvantix.Person.Features.PersonInfo;

public sealed class PersonInfoModelValidator : AbstractValidator<PersonInfoModel>
{
    public PersonInfoModelValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().WithMessage("AccountId является обязательным полем");

        RuleFor(x => x.Gender).IsInEnum().WithMessage("Указан некорректный пол");
    }
}
