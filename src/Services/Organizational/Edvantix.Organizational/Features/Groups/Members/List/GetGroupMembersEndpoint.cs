namespace Edvantix.Organizational.Features.Groups.Members.List;

public sealed class GetGroupMembersEndpoint
    : IEndpoint<Ok<PagedResult<GroupMemberDto>>, GetGroupMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/groups/{groupId:guid}/members",
                async (
                    Guid groupId,
                    [AsParameters] GetGroupMembersQueryParams query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                    await HandleAsync(
                        new GetGroupMembersQuery(
                            groupId,
                            query.IncludeExited,
                            query.PageIndex,
                            query.PageSize
                        ),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("GetGroupMembers")
            .WithTags("Участники группы")
            .WithSummary("Получить постраничный список участников группы")
            .WithPaginationHeaders()
            .Produces<PagedResult<GroupMemberDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<GroupMemberDto>>> HandleAsync(
        GetGroupMembersQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}

/// <summary>Query-параметры для получения списка участников группы.</summary>
public sealed record GetGroupMembersQueryParams(
    [property: Description("Включить выбывших участников")] bool IncludeExited = false,
    [property: Description("Индекс страницы")] int PageIndex = Pagination.DefaultPageIndex,
    [property: Description("Количество элементов на странице")] int PageSize = Pagination.DefaultPageSize
);
