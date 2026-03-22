namespace Edvantix.Persona.Features.Profiles.Details;

public sealed class GetProfileDetailsEndpoint : IEndpoint<Ok<ProfileDetailsModel>, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profile/details",
                async (ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(sender, cancellationToken)
            )
            .WithName("Получение полного профиля")
            .WithTags("Профиль")
            .WithSummary("Получить полный профиль")
            .WithDescription(
                "Возвращает полную информацию о профиле текущего пользователя, "
                    + "включая контакты, образование и опыт работы"
            )
            .Produces<ProfileDetailsModel>()
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .MapToApiVersion(ApiVersions.V1)
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
