namespace Edvantix.Persona.Features.Profiles.Details;

/// <summary>GET /v1/profile/details — полный профиль текущего пользователя.</summary>
public sealed class GetProfileDetailsEndpoint : IEndpoint<Ok<ProfileDetailsModel>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profile/details",
                async (ISender sender, CancellationToken ct) => await HandleAsync(sender, ct)
            )
            .WithName("GetProfileDetails")
            .WithTags("Profile")
            .WithSummary("Получить полный профиль")
            .WithDescription(
                "Возвращает полную информацию о профиле текущего пользователя, "
                    + "включая контакты, образование и опыт работы"
            )
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(new(1, 0))
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileDetailsModel>> HandleAsync(
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetProfileDetailsQuery(), cancellationToken);
        return TypedResults.Ok(result);
    }
}
