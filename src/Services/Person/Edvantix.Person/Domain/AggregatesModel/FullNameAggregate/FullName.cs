using Edvantix.Chassis.EF.Attributes;
using Edvantix.Chassis.Specification;
using Edvantix.Person.Domain.AggregatesModel.PersonInfoAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Person.Domain.AggregatesModel.FullNameAggregate;

public sealed class FullName() : Entity<long>, ISoftDelete, IAggregateRoot
{
    internal FullName(string firstName, string lastName, string? middleName = null)
        : this()
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Имя не может быть пустым.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Фамилия не может быть пустой.", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    public long PersonInfoId { get; private set; }
    public PersonInfo PersonInfo { get; private set; } = null!;

    [OrderBy(OrderType.ThenBy, 1)]
    public string FirstName { get; private set; } = null!;

    [OrderBy]
    public string LastName { get; private set; } = null!;

    [OrderBy(OrderType.ThenBy, 2)]
    public string? MiddleName { get; private set; }

    public string GetFullName() => $"{LastName} {FirstName} {MiddleName}".TrimEnd();

    internal void Update(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Имя не может быть пустым.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Фамилия не может быть пустой.", nameof(lastName));

        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    public bool IsDeleted { get; set; }

    public void Delete()
    {
        IsDeleted = true;
    }
}
