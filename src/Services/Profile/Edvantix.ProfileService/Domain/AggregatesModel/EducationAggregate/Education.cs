using Edvantix.Chassis.EF.Attributes;
using Edvantix.Chassis.Specification;
using Edvantix.ProfileService.Domain.Abstractions;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.ProfileService.Domain.AggregatesModel.EducationAggregate;

public sealed class Education() : PersonalData<long>, ISoftDelete, IAggregateRoot
{
    internal Education(
        DateTime dateStart,
        string institution,
        EducationLevel educationLevel,
        string? specialty = null,
        DateTime? dateEnd = null
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

    [OrderBy]
    public DateTime DateStart { get; private set; }

    [OrderBy(OrderType.ThenBy)]
    public DateTime? DateEnd { get; private set; }

    public string Institution { get; private set; } = null!;
    public string? Specialty { get; private set; }

    public bool IsDeleted { get; set; }

    public EducationLevel EducationLevel { get; private set; }

    internal void Update(
        DateTime dateStart,
        DateTime? dateEnd,
        string institution,
        string? specialty,
        EducationLevel educationLevel
    )
    {
            DateStart = dateStart;

        if (dateEnd.HasValue)
        {
            if (dateEnd.Value < DateStart)
                throw new ArgumentException("Дата окончания не может быть раньше даты начала.");
            DateEnd = dateEnd.Value;
        }

        if (string.IsNullOrWhiteSpace(institution))
            throw new ArgumentException(
                "Название учебного заведения не может быть пустым.",
                nameof(institution)
            );
        Institution = institution;

        Specialty = specialty;

        EducationLevel = educationLevel;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
