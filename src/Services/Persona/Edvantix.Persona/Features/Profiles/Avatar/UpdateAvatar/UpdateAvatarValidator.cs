namespace Edvantix.Persona.Features.Profiles.Avatar.UpdateAvatar;

public sealed class UpdateAvatarValidator : AbstractValidator<UpdateAvatarCommand>
{
    public UpdateAvatarValidator()
    {
        RuleFor(x => x.Avatar).ApplyImageRules();
    }
}
