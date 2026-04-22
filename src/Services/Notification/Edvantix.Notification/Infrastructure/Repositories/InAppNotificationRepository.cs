using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Notification.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Notification.Infrastructure.Repositories;

/// <summary>
/// EF Core реализация репозитория in-app уведомлений.
/// </summary>
public sealed class InAppNotificationRepository(NotificationDbContext context)
    : IInAppNotificationRepository
{
    private readonly NotificationDbContext _context =
        context ?? throw new ArgumentNullException(nameof(context));

    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => _context;

    /// <inheritdoc />
    public async Task AddAsync(
        InAppNotification notification,
        CancellationToken cancellationToken = default
    )
    {
        await _context.InAppNotifications.AddAsync(notification, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InAppNotification?> FindAsync(
        ISpecification<InAppNotification> spec,
        CancellationToken cancellationToken = default
    )
    {
        return await Specification
            .GetQuery(_context.InAppNotifications, spec)
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IReadOnlyList<InAppNotification> Items, int TotalCount)> ListPagedAsync(
        ISpecification<InAppNotification> spec,
        ISpecification<InAppNotification> countSpec,
        CancellationToken cancellationToken = default
    )
    {
        var items = await Specification
            .GetQuery(_context.InAppNotifications, spec)
            .ToListAsync(cancellationToken);

        var totalCount = await Specification
            .GetQuery(_context.InAppNotifications, countSpec)
            .CountAsync(cancellationToken);

        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        ISpecification<InAppNotification> spec,
        CancellationToken cancellationToken = default
    )
    {
        return await Specification
            .GetQuery(_context.InAppNotifications, spec)
            .CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task MarkAllAsReadAsync(
        Guid profileId,
        CancellationToken cancellationToken = default
    )
    {
        // Эффективное bulk-обновление через ExecuteUpdateAsync (EF Core 7+)
        await _context
            .InAppNotifications.Where(n => n.ProfileId == profileId && !n.IsRead)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(n => n.IsRead, true)
                        .SetProperty(n => n.ReadAt, DateTime.UtcNow),
                cancellationToken
            );
    }
}
