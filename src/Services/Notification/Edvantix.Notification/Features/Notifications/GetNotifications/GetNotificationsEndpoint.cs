namespace Edvantix.Notification.Features.Notifications.GetNotifications;

/// <summary>
/// Запрос с параметрами пагинации и фильтрации (без AccountId — он приходит из JWT).
/// </summary>
public sealed record GetNotificationsRequest(
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: DefaultValue(Pagination.DefaultPageSize)] int PageSize = Pagination.DefaultPageSize,
    bool? IsRead = null
);

/// <summary>
/// GET /notifications — пагинированный список in-app уведомлений текущего пользователя.
/// </summary>
public sealed class GetNotificationsEndpoint
    : IEndpoint<Ok<PagedResult<NotificationViewModel>>, GetNotificationsQuery, ISender>
{
    /// <inheritdoc />
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/notifications",
                async (
                    ClaimsPrincipal user,
                    [AsParameters] GetNotificationsRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                    await HandleAsync(
                        new GetNotificationsQuery(
                            NotificationHelpers.GetAccountId(user),
                            request.PageIndex,
                            request.PageSize,
                            request.IsRead
                        ),
                        sender,
                        ct
                    )
            )
            .WithName("GetNotifications")
            .WithTags("Notifications")
            .WithSummary("Список уведомлений")
            .WithDescription(
                "Возвращает пагинированный список in-app уведомлений текущего пользователя."
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<NotificationViewModel>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    /// <inheritdoc />
    public async Task<Ok<PagedResult<NotificationViewModel>>> HandleAsync(
        GetNotificationsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
