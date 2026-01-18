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
        long educationLevelId,
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
        EducationLevelId = educationLevelId;
    }

    [OrderBy]
    public DateTime DateStart { get; private set; }

    [OrderBy(OrderType.ThenBy)]
    public DateTime? DateEnd { get; private set; }

    public string Institution { get; private set; } = null!;
    public string? Specialty { get; private set; }
    public long EducationLevelId { get; private set; }

    public bool IsDeleted { get; set; }

    [Include]
    public EducationLevel EducationLevel { get; private set; } = null!;

    internal void Update(
        DateTime? dateStart = null,
        DateTime? dateEnd = null,
        string? institution = null,
        string? specialty = null,
        long? educationLevelId = null
    )
    {
        if (dateStart.HasValue)
            DateStart = dateStart.Value;

        if (dateEnd.HasValue)
        {
            if (dateEnd.Value < DateStart)
                throw new ArgumentException("Дата окончания не может быть раньше даты начала.");
            DateEnd = dateEnd.Value;
        }

        if (institution != null)
        {
            if (string.IsNullOrWhiteSpace(institution))
                throw new ArgumentException(
                    "Название учебного заведения не может быть пустым.",
                    nameof(institution)
                );
            Institution = institution;
        }

        if (specialty != null)
            Specialty = specialty;

        if (educationLevelId.HasValue)
            EducationLevelId = educationLevelId.Value;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
