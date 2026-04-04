namespace Edvantix.Persona.Features.Admin.Profiles.List;

public sealed class GetAdminProfilesEndpoint
    : IEndpoint<Ok<PagedResult<AdminProfileDto>>, GetAdminProfilesQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/admin/profiles",
                async (
                    [AsParameters] GetAdminProfilesQuery request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("Получить список профилей (админ)")
            .WithTags("Администрирование")
            .WithSummary("Постраничный список всех профилей")
            .WithDescription("Возвращает список профилей с пагинацией, поиском и фильтрацией")
            .WithPaginationHeaders()
            .Produces<PagedResult<AdminProfileDto>>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<PagedResult<AdminProfileDto>>> HandleAsync(
        GetAdminProfilesQuery request,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return TypedResults.Ok(result);
    }
}
