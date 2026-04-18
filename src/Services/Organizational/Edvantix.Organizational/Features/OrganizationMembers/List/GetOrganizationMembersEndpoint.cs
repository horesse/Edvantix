namespace Edvantix.Organizational.Features.OrganizationMembers.List;

public sealed class GetOrganizationMembersEndpoint
    : IEndpoint<Ok<PagedResult<OrganizationMemberDto>>, GetOrganizationMembersQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/members",
                async (
                    [AsParameters] GetOrganizationMembersQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("GetOrganizationMembers")
            .WithTags("Участники организации")
            .WithSummary("Получить список участников организации")
            .WithDescription(
                "Возвращает постраничный список участников организации с фильтрацией по статусу"
            )
            .WithPaginationHeaders()
            .Produces<PagedResult<OrganizationMemberDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization();
    }

    public async Task<Ok<PagedResult<OrganizationMemberDto>>> HandleAsync(
        GetOrganizationMembersQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
