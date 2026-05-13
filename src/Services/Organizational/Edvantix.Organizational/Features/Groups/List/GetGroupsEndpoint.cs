namespace Edvantix.Organizational.Features.Groups.List;

public sealed class GetGroupsEndpoint
    : IEndpoint<Ok<PagedResult<GroupListItemDto>>, GetGroupsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups",
                async (
                    [AsParameters] GetGroupsQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("GetGroups")
            .WithTags("Группы")
            .WithSummary("Получить постраничный список групп организации")
            .WithDescription(
                "Поддерживает фильтрацию по уровню, статусу, формату и текстовый поиск по названию"
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<GroupListItemDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<GroupListItemDto>>> HandleAsync(
        GetGroupsQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
