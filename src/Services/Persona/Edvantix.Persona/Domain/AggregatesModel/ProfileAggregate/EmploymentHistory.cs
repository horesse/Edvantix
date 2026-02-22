namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

public sealed class EmploymentHistory() : PersonalData
{
    internal EmploymentHistory(
        string workplace,
        string position,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null
    )
        : this()
    {
        if (string.IsNullOrWhiteSpace(workplace))
            throw new ArgumentException(
                "Название компании не может быть пустым.",
                nameof(workplace)
            );

        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Должность не может быть пустой.", nameof(position));

        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("Дата окончания не может быть раньше даты начала.");

        Workplace = workplace;
        Position = position;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }

    public string Workplace { get; private set; } = null!;
    public string Position { get; private set; } = null!;

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }
    public string? Description { get; private set; }

    public bool IsDeleted { get; set; }
}
