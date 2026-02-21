namespace Edvantix.Persona.Features.Profiles.OwnProfile;

public class GetOwnProfileEndpoint : IEndpoint<Ok<ProfileViewModel>, GetOwnProfileQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profile",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetOwnProfileQuery(), sender, ct)
            )
            .WithName("GetOwnProfile")
            .WithTags("Profile")
            .WithSummary("Получить собственный профиль")
            .WithDescription("Возвращает информацию о профиле текущего пользователя")
            .Produces<ProfileViewModel>()
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileViewModel>> HandleAsync(
        GetOwnProfileQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
