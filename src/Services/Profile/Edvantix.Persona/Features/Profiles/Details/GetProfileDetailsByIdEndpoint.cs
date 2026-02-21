namespace Edvantix.Persona.Features.Profiles.Details;

/// <summary>GET /v1/profiles/{id}/details — полный профиль по ID (только для администратора).</summary>
public sealed class GetProfileDetailsByIdEndpoint
    : IEndpoint<Ok<ProfileDetailsModel>, GetProfileDetailsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profiles/{id:long}/details",
                async (ulong id, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetProfileDetailsQuery(id), sender, ct)
            )
            .WithName("GetProfileDetailsByAdmin")
            .WithTags("Profile")
            .WithSummary("Получить полный профиль по ID (администратор)")
            .WithDescription("Возвращает полную информацию о профиле любого пользователя")
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        GetProfileDetailsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
