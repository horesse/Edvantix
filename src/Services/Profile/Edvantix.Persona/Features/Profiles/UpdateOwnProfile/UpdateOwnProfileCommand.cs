using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>Команда обновления собственного профиля. Возвращает обновлённое краткое представление.</summary>
public sealed record UpdateOwnProfileCommand(UpdateProfileRequest Request)
    : IRequest<ProfileViewModel>;

public sealed class UpdateOwnProfileCommandHandler(IServiceProvider provider)
    : IRequestHandler<UpdateOwnProfileCommand, ProfileViewModel>
{
    public async ValueTask<ProfileViewModel> Handle(
        UpdateOwnProfileCommand command,
        CancellationToken ct
    )
    {
        var accountId = provider.GetUserId();
        var profileRepo = provider.GetRequiredService<IProfileRepository>();

        // Загружаем с коллекциями, так как ReplaceContacts/Educations/EmploymentHistories
        // требуют загруженных навигационных свойств для каскадного удаления EF Core
        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        ApplyChanges(profile, command.Request);

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileViewModel>>();
        return mapper.Map(profile);
    }

    /// <summary>Применяет изменения из запроса к агрегату Profile.</summary>
    private static void ApplyChanges(Profile profile, UpdateProfileRequest request)
    {
        profile.UpdatePersonalInfo(request.Gender, request.BirthDate);
        profile.UpdateFullName(request.FirstName, request.LastName, request.MiddleName);

        profile.ReplaceContacts(
            request.Contacts.Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
        );

        profile.ReplaceEducations(
            request.Educations.Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        profile.ReplaceEmploymentHistories(
            request.EmploymentHistories.Select(e =>
                profile.CreateEmploymentHistory(
                    e.Workplace,
                    e.Position,
                    e.StartDate,
                    e.EndDate,
                    e.Description
                )
            )
        );
    }
}
