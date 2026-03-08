namespace Edvantix.Persona.Features.Profiles.UpdateAvatar;

/// <summary>Валидатор команды загрузки аватара.</summary>
public sealed class UpdateAvatarValidator : AbstractValidator<UpdateAvatarCommand>
{
    public UpdateAvatarValidator()
    {
        RuleFor(x => x.Avatar).SetValidator(new ImageValidator());
    }
}
