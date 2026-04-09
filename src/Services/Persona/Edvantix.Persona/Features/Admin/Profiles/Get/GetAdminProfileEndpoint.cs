namespace Edvantix.Persona.Features.Admin.Profiles.Get;

public sealed class GetAdminProfileEndpoint : IEndpoint<Ok<AdminProfileDetailDto>, Guid, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/admin/profiles/{profileId:guid}",
                async (Guid profileId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(profileId, sender, cancellationToken)
            )
            .WithName("Получить профиль (админ)")
            .WithTags("Администрирование")
            .WithSummary("Детальная информация о профиле")
            .WithDescription(
                "Возвращает полную информацию о профиле пользователя для редактирования администратором."
            )
            .Produces<AdminProfileDetailDto>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden)
            .MapToApiVersion(ApiVersions.V1)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Ok<AdminProfileDetailDto>> HandleAsync(
        Guid profileId,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(new GetAdminProfileQuery(profileId), cancellationToken);
        return TypedResults.Ok(result);
    }
}
