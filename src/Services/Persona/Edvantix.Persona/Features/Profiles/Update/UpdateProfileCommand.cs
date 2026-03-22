using Edvantix.Constants.Other;

namespace Edvantix.Persona.Features.Profiles.Update;

public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string? MiddleName,
    DateOnly BirthDate,
    string? Bio,
    List<ContactRequest> Contacts,
    List<EmploymentHistoryRequest> EmploymentHistories,
    List<EducationRequest> Educations,
    List<string> Skills
) : ICommand<Guid>;

public sealed record ContactRequest(ContactType Type, string Value, string? Description = null);

public sealed record EducationRequest(
    DateOnly DateStart,
    string Institution,
    EducationLevel Level,
    string? Specialty = null,
    DateOnly? DateEnd = null
);

public sealed record EmploymentHistoryRequest(
    string Workplace,
    string Position,
    DateTime StartDate,
    DateTime? EndDate = null,
    string? Description = null
);

public sealed class UpdateProfileCommandHandler(
    ClaimsPrincipal claims,
    IProfileRepository repository,
    ISkillRepository skillRepository
) : ICommandHandler<UpdateProfileCommand, Guid>
{
    public async ValueTask<Guid> Handle(
        UpdateProfileCommand command,
        CancellationToken cancellationToken
    )
    {
        var profileId = claims.GetProfileIdOrError();

        var spec = ProfileSpecification.ForWrite(profileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, profileId);

        // Обновляем личные данные
        profile.UpdatePersonalInfo(command.BirthDate);
        profile.UpdateFullName(command.FirstName, command.LastName, command.MiddleName);
        profile.UpdateBio(command.Bio);

        // Заменяем коллекции контактов, опыта и образования
        profile.ReplaceContacts(
            command.Contacts.Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
        );
        profile.ReplaceEmploymentHistories(
            command.EmploymentHistories.Select(e =>
                profile.CreateEmploymentHistory(
                    e.Workplace,
                    e.Position,
                    e.StartDate,
                    e.EndDate,
                    e.Description
                )
            )
        );
        profile.ReplaceEducations(
            command.Educations.Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        // Разрешаем имена навыков в ID каталога (find-or-create), затем заменяем список
        var skillIds = await ResolveSkillIdsAsync(
            command.Skills,
            skillRepository,
            cancellationToken
        );
        profile.ReplaceSkills(skillIds);

        await repository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

        return profileId;
    }

    /// <summary>
    /// Разрешает список названий навыков в ID из глобального каталога.
    /// Навыки, которых ещё нет в каталоге, создаются автоматически.
    /// Дубликаты (без учёта регистра) схлопываются в одну запись.
    /// </summary>
    private static async Task<List<Guid>> ResolveSkillIdsAsync(
        List<string> names,
        ISkillRepository skillRepo,
        CancellationToken cancellationToken
    )
    {
        var skillIds = new List<Guid>(names.Count);

        // Дедупликация по нормализованному имени (регистронезависимо) до обращений к БД
        var uniqueNames = names
            .Select(n => n.Trim())
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var name in uniqueNames)
        {
            var spec = new SkillSpecification(name);

            var skill =
                await skillRepo.FindAsync(spec, cancellationToken)
                ?? await skillRepo.AddAsync(new Skill(name), cancellationToken);

            skillIds.Add(skill.Id);
        }

        return skillIds;
    }
}
