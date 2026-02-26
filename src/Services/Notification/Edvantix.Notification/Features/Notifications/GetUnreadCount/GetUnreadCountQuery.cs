namespace Edvantix.Notification.Features.Notifications.GetUnreadCount;

/// <summary>
/// ViewModel для количества непрочитанных уведомлений.
/// </summary>
public sealed record UnreadCountViewModel(int Count);

/// <summary>
/// Запрос на получение количества непрочитанных уведомлений (для бейджа в шапке).
/// </summary>
public sealed record GetUnreadCountQuery(Guid AccountId) : IRequest<UnreadCountViewModel>;

/// <summary>
/// Обработчик запроса: подсчитывает непрочитанные уведомления пользователя.
/// </summary>
public sealed class GetUnreadCountQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetUnreadCountQuery, UnreadCountViewModel>
{
    /// <inheritdoc />
    public async ValueTask<UnreadCountViewModel> Handle(
        GetUnreadCountQuery request,
        CancellationToken cancellationToken
    )
    {
        var repo = provider.GetRequiredService<IInAppNotificationRepository>();
        var spec = new InAppNotificationsCountSpec(request.AccountId, isRead: false);
        var count = await repo.CountAsync(spec, cancellationToken);

        return new UnreadCountViewModel(count);
    }
}
