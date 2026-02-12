using FluentValidation;

namespace Edvantix.ProfileService.Features.UserContactFeature.Features.UpdateOwnContacts;

/// <summary>
/// Валидатор команды обновления контактов
/// </summary>
public sealed class UpdateOwnContactsValidator : AbstractValidator<UpdateOwnContactsCommand>
{
    public UpdateOwnContactsValidator()
    {
        RuleForEach(x => x.Contacts).SetValidator(new Validator());
    }
}
