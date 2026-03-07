using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdatePersonalInfo;

/// <summary>PATCH /v1/profile/personal-info — обновить личную информацию.</summary>
public sealed record UpdatePersonalInfoCommand(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly BirthDate
) : IRequest<ProfileDetailsModel>;

public sealed class UpdatePersonalInfoCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdatePersonalInfoCommand, ProfileDetailsModel>
{
    public async ValueTask<ProfileDetailsModel> Handle(
        UpdatePersonalInfoCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        profile.UpdateFullName(command.FirstName, command.LastName, command.MiddleName);
        profile.UpdatePersonalInfo(command.BirthDate);

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileDetailsModel>>();
        return mapper.Map(profile);
    }
}
