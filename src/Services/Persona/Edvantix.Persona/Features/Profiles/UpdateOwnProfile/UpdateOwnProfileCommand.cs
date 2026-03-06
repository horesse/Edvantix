using Edvantix.Chassis.Utilities;

namespace Edvantix.Persona.Features.Profiles.UpdateOwnProfile;

/// <summary>
/// Команда обновления собственного профиля. Принимается как multipart/form-data.
/// Возвращает обновлённое краткое представление профиля.
/// </summary>
public sealed class UpdateOwnProfileCommand : IRequest<ProfileViewModel>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? MiddleName { get; init; }
    public DateOnly BirthDate { get; init; }

    /// <summary>Полный список контактов. Заменяет все существующие.</summary>
    public List<ContactRequest>? Contacts { get; init; } = [];

    /// <summary>Полный список образования. Заменяет все существующие записи.</summary>
    public List<EducationRequest>? Educations { get; init; } = [];

    /// <summary>Полная история занятости. Заменяет все существующие записи.</summary>
    public List<EmploymentHistoryRequest>? EmploymentHistories { get; init; } = [];

    /// <summary>Новый аватар пользователя (необязательно, JPEG/PNG до 1 МБ).</summary>
    public IFormFile? Avatar { get; init; }

    /// <summary>URN загруженного аватара. Устанавливается PreProcessor'ом перед выполнением команды.</summary>
    public string? AvatarUrn { get; set; }

    /// <summary>URN предыдущего аватара. Устанавливается обработчиком для удаления PostProcessor'ом.</summary>
    public string? OldAvatarUrn { get; set; }
}

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

        // Загружаем с коллекциями, так как Replace* требуют загруженных навигационных свойств
        var spec = new ProfileByAccountIdSpec(accountId, withDetails: true);
        var profile =
            await profileRepo.FindAsync(spec, ct)
            ?? throw new NotFoundException("Профиль не найден.");

        if (command.AvatarUrn is not null)
        {
            // Сохраняем URN старого аватара для удаления в PostProcessor после успешного сохранения
            command.OldAvatarUrn = profile.AvatarUrl;
            profile.UploadAvatar(command.AvatarUrn);
        }

        profile.UpdatePersonalInfo(command.BirthDate);
        profile.UpdateFullName(command.FirstName, command.LastName, command.MiddleName);

        // Коллекции могут быть null, если multipart/form-data не содержит ни одного элемента —
        // model binder не может различить "пустой список" и "поле не передано".
        profile.ReplaceContacts(
            (command.Contacts ?? []).Select(c => profile.CreateContact(c.Type, c.Value, c.Description))
        );

        profile.ReplaceEducations(
            (command.Educations ?? []).Select(e =>
                profile.CreateEducation(e.DateStart, e.Institution, e.Level, e.Specialty, e.DateEnd)
            )
        );

        profile.ReplaceEmploymentHistories(
            (command.EmploymentHistories ?? []).Select(e =>
                profile.CreateEmploymentHistory(
                    e.Workplace,
                    e.Position,
                    e.StartDate,
                    e.EndDate,
                    e.Description
                )
            )
        );

        await profileRepo.UnitOfWork.SaveEntitiesAsync(ct);

        var mapper = provider.GetRequiredService<IMapper<Profile, ProfileViewModel>>();
        return mapper.Map(profile);
    }
}
