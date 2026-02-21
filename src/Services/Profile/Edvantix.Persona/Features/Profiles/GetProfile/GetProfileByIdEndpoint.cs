namespace Edvantix.Persona.Features.Profiles.GetProfile;

/// <summary>GET /v1/profiles/{id} — краткий профиль по ID (только для администратора).</summary>
public sealed class GetProfileByIdEndpoint
    : IEndpoint<Ok<ProfileViewModel>, GetProfileQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profiles/{id:long}",
                async (ulong id, ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetProfileQuery(id), sender, ct)
            )
            .WithName("GetProfileById")
            .WithTags("Profile")
            .WithSummary("Получить профиль по ID (администратор)")
            .WithDescription("Возвращает краткую информацию о профиле любого пользователя")
            .Produces<ProfileViewModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<ProfileViewModel>> HandleAsync(
        GetProfileQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
