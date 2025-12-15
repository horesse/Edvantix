using Edvantix.OrganizationManagement.Domain.AggregatesModel.OrganizationAggregate;
using Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.SubscriptionAggregate;

public sealed class Subscription() : Entity<long>, IAggregateRoot
{
    private readonly List<Usage> _usages = new();

    public Subscription(
        long subscriptionId,
        long organizationId,
        DateTime dateStart,
        DateTime? dateEnd = null
    )
        : this()
    {
        if (subscriptionId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор подписки.",
                nameof(subscriptionId)
            );

        if (organizationId <= 0)
            throw new ArgumentException(
                "Некорректный идентификатор организации.",
                nameof(organizationId)
            );

        if (dateEnd.HasValue && dateEnd.Value < dateStart)
            throw new ArgumentException(
                "Дата окончания не может быть раньше даты начала.",
                nameof(dateEnd)
            );

        SubscriptionId = subscriptionId;
        OrganizationId = organizationId;
        DateStart = dateStart;
        DateEnd = dateEnd;
    }

    public long SubscriptionId { get; private set; }
    public long OrganizationId { get; private set; }
    public Organization Organization { get; private set; } = null!;
    public DateTime DateStart { get; private set; }
    public DateTime? DateEnd { get; private set; }

    public IReadOnlyCollection<Usage> Usages => _usages.AsReadOnly();

    public void ExtendSubscription(DateTime newEndDate)
    {
        if (newEndDate < DateStart)
            throw new ArgumentException(
                "Дата окончания не может быть раньше даты начала.",
                nameof(newEndDate)
            );

        if (DateEnd.HasValue && newEndDate < DateEnd.Value)
            throw new ArgumentException(
                "Невозможно сократить подписку. Используйте CancelSubscription.",
                nameof(newEndDate)
            );

        DateEnd = newEndDate;
    }

    public void CancelSubscription(DateTime cancellationDate)
    {
        if (cancellationDate < DateStart)
            throw new ArgumentException(
                "Дата отмены не может быть раньше даты начала.",
                nameof(cancellationDate)
            );

        DateEnd = cancellationDate;
    }

    public void AddUsage(long limitId, decimal value)
    {
        var usage = new Usage(OrganizationId, limitId, value);
        _usages.Add(usage);
    }

    public void RemoveUsage(long usageId)
    {
        var usage = _usages.FirstOrDefault(u => u.Id == usageId);
        if (usage == null)
            throw new InvalidOperationException($"Использование с ID {usageId} не найдено.");

        _usages.Remove(usage);
    }

    public bool IsActive() => !DateEnd.HasValue || DateEnd.Value >= DateTime.UtcNow;
}
