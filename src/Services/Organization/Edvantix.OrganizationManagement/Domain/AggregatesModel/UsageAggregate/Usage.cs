using Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;

public sealed class Usage() : Entity<long>, IAggregateRoot
{
    public Usage(long organizationId, long limitId, decimal value)
        : this()
    {
        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );
        
        if (limitId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор лимита.",
                nameof(limitId)
            );
        
        if (value < 0)
            throw new ArgumentException(
                "Значение использования не может быть отрицательным.",
                nameof(value)
            );

        OrganizationId = organizationId;
        LimitId = limitId;
        Value = value;
    }

    public long OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public long LimitId { get; private set; }
    public decimal Value { get; private set; }

    public void UpdateValue(decimal newValue)
    {
        if (newValue < 0)
            throw new ArgumentException(
                "Значение использования не может быть отрицательным.",
                nameof(newValue)
            );

        Value = newValue;
    }

    public void IncrementValue(decimal increment)
    {
        if (increment <= 0)
            throw new ArgumentException(
                "Инкремент должен быть положительным.",
                nameof(increment)
            );

        Value += increment;
    }

    public void DecrementValue(decimal decrement)
    {
        if (decrement <= 0)
            throw new ArgumentException(
                "Декремент должен быть положительным.",
                nameof(decrement)
            );
        
        if (Value - decrement < 0)
            throw new InvalidOperationException("Невозможно уменьшить значение ниже нуля.");

        Value -= decrement;
    }
}
