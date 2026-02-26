using Edvantix.Notification.Features.Notifications;

namespace Edvantix.Notification.Features.Notifications.GetNotifications;

/// <summary>
/// Запрос на получение страницы in-app уведомлений пользователя.
/// </summary>
public sealed record GetNotificationsQuery(
    [property: Description("Идентификатор аккаунта Keycloak")] Guid AccountId,
    [property: Description("Индекс страницы")]
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")]
    [property: DefaultValue(Pagination.DefaultPageSize)]
        int PageSize = Pagination.DefaultPageSize,
    [property: Description("Фильтр по статусу прочтения")] bool? IsRead = null
) : IRequest<PagedResult<NotificationViewModel>>;

/// <summary>
/// Обработчик запроса: возвращает пагинированный список уведомлений.
/// </summary>
public sealed class GetNotificationsQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetNotificationsQuery, PagedResult<NotificationViewModel>>
{
    /// <inheritdoc />
    public async ValueTask<PagedResult<NotificationViewModel>> Handle(
        GetNotificationsQuery request,
        CancellationToken cancellationToken
    )
    {
        var clamped = (
            PageIndex: Math.Max(request.PageIndex, 1),
            PageSize: Math.Clamp(request.PageSize, 1, 100)
        );

        var repo = provider.GetRequiredService<IInAppNotificationRepository>();

        var listSpec = new InAppNotificationsByAccountSpec(
            request.AccountId,
            clamped.PageIndex,
            clamped.PageSize,
            request.IsRead
        );

        var countSpec = new InAppNotificationsCountSpec(request.AccountId, request.IsRead);

        var (items, totalCount) = await repo.ListPagedAsync(listSpec, countSpec, cancellationToken);

        var viewModels = items.Select(NotificationViewModel.FromDomain).ToList();

        return new PagedResult<NotificationViewModel>(
            viewModels,
            clamped.PageIndex,
            clamped.PageSize,
            totalCount
        );
    }
}
