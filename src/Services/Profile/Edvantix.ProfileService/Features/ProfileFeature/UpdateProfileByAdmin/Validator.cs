using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Features.ProfileFeature.UpdateOwnProfile;
using FluentValidation;

namespace Edvantix.ProfileService.Features.ProfileFeature.UpdateProfileByAdmin;

/// <summary>
/// Валидатор для команды обновления профиля администратором
/// </summary>
public sealed class UpdateProfileByAdminCommandValidator
    : AbstractValidator<UpdateProfileByAdminCommand>
{
    public UpdateProfileByAdminCommandValidator()
    {
        RuleFor(x => x.ProfileId).GreaterThan(0).WithMessage("ID профиля должен быть больше нуля");

        RuleFor(x => x.Profile).NotNull().SetValidator(new UpdateProfileModelValidator());
    }
}
