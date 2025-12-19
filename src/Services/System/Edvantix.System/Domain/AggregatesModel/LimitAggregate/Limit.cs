using Edvantix.SharedKernel.SeedWork;
using Edvantix.System.Domain.AggregatesModel.SubscriptionAggregate;

namespace Edvantix.System.Domain.AggregatesModel.LimitAggregate;

public sealed class Limit() : Entity<long>, IAggregateRoot
{
    public Limit(long subscriptionId, LimitType type, decimal value)
        : this()
    {
        if (subscriptionId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор подписки.",
                nameof(subscriptionId)
            );

        if (value < 0)
            throw new ArgumentException("Ограничение не может быть отрицательным.", nameof(value));

        SubscriptionId = subscriptionId;
        Type = type;
        Value = value;
    }

    public long SubscriptionId { get; private set; }
    public Subscription Subscription { get; set; } = null!;

    public LimitType Type { get; private set; }
    public decimal Value { get; private set; }

    public void UpdateValue(decimal newValue)
    {
        if (newValue < 0)
            throw new ArgumentException("Limit value cannot be negative.", nameof(newValue));

        Value = newValue;
    }

    public void ChangeType(LimitType newType)
    {
        Type = newType;
    }
}
