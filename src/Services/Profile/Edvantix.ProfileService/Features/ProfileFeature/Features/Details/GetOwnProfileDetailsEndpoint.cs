using Edvantix.Chassis.Endpoints;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using Edvantix.ProfileService.Features.ProfileFeature.OwnProfile;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.ProfileFeature.Features.Details;

/// <summary>
/// Эндпоинт для получения полной информации профиля текущего пользователя
/// </summary>
public class GetOwnProfileDetailsEndpoint
    : IEndpoint<Ok<ProfileModel>, GetOwnProfileDetailsQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/profile/details",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetOwnProfileDetailsQuery(), sender, ct)
            )
            .WithName("GetOwnProfileDetails")
            .WithTags("Profile")
            .WithSummary("Получить полную информацию собственного профиля")
            .WithDescription(
                "Возвращает полную информацию о профиле текущего пользователя, включая контакты, образование и опыт работы"
            )
            .Produces<ProfileModel>()
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization();
    }

    public async Task<Ok<ProfileModel>> HandleAsync(
        GetOwnProfileDetailsQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
