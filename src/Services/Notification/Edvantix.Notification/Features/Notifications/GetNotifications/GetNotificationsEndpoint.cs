namespace Edvantix.Notification.Features.Notifications.GetNotifications;

public sealed record GetNotificationsRequest(
    [property: DefaultValue(Pagination.DefaultPageIndex)]
        int PageIndex = Pagination.DefaultPageIndex,
    [property: DefaultValue(Pagination.DefaultPageSize)] int PageSize = Pagination.DefaultPageSize,
    bool? IsRead = null
);

public sealed class GetNotificationsEndpoint
    : IEndpoint<Ok<PagedResult<NotificationViewModel>>, GetNotificationsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/notifications",
                async (
                    ClaimsPrincipal user,
                    [AsParameters] GetNotificationsRequest request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new GetNotificationsQuery(
                            NotificationHelpers.GetAccountId(user),
                            request.PageIndex,
                            request.PageSize,
                            request.IsRead
                        ),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("Список уведомлений")
            .WithTags("Уведомления")
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
