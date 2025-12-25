using Edvantix.Chassis.EF.Attributes;
using Edvantix.Person.Domain.Abstractions;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;

public sealed class EmploymentHistory() : PersonalData<long>, ISoftDelete, IAggregateRoot
{
    internal EmploymentHistory(
        string companyName,
        string position,
        DateTime startDate,
        DateTime? endDate = null,
        string? description = null
    )
        : this()
    {
        if (string.IsNullOrWhiteSpace(companyName))
            throw new ArgumentException(
                "Название компании не может быть пустым.",
                nameof(companyName)
            );

        if (string.IsNullOrWhiteSpace(position))
            throw new ArgumentException("Должность не может быть пустой.", nameof(position));

        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("Дата окончания не может быть раньше даты начала.");

        CompanyName = companyName;
        Position = position;
        StartDate = startDate;
        EndDate = endDate;
        Description = description;
    }

    public string CompanyName { get; private set; } = null!;
    public string Position { get; private set; } = null!;

    [OrderBy]
    public DateTime StartDate { get; private set; }
    public DateTime? EndDate { get; private set; }
    public string? Description { get; private set; }

    public bool IsDeleted { get; set; }

    internal void Update(
        string? companyName = null,
        string? position = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? description = null
    )
    {
        if (companyName != null)
        {
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException(
                    "Название компании не может быть пустым.",
                    nameof(companyName)
                );
            CompanyName = companyName;
        }

        if (position != null)
        {
            if (string.IsNullOrWhiteSpace(position))
                throw new ArgumentException("Должность не может быть пустой.", nameof(position));
            Position = position;
        }

        if (startDate.HasValue)
            StartDate = startDate.Value;

        if (endDate.HasValue)
        {
            if (endDate.Value < StartDate)
                throw new ArgumentException("Дата окончания не может быть раньше даты начала.");
            EndDate = endDate.Value;
        }

        if (description != null)
            Description = description;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
