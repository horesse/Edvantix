namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class Education() : PersonalData
{
    internal Education(
        DateOnly dateStart,
        string institution,
        EducationLevel educationLevel,
        string? specialty = null,
        DateOnly? dateEnd = null
    )
        : this()
    {
        if (string.IsNullOrWhiteSpace(institution))
            throw new ArgumentException(
                "Название учебного заведения не может быть пустым.",
                nameof(institution)
            );

        if (dateEnd.HasValue && dateEnd.Value < dateStart)
            throw new ArgumentException("Дата окончания не может быть раньше даты начала.");

        DateStart = dateStart;
        DateEnd = dateEnd;
        Institution = institution;
        Specialty = specialty;
        EducationLevel = educationLevel;
    }

    public DateOnly DateStart { get; private set; }

    public DateOnly? DateEnd { get; private set; }

    public string Institution { get; private set; } = null!;
    public string? Specialty { get; private set; }

    public bool IsDeleted { get; set; }

    public EducationLevel EducationLevel { get; private set; }
}
