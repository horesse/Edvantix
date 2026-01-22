using Edvantix.Chassis.Endpoints;
using Edvantix.ProfileService.Features.ProfileFeature.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.ProfileFeature.OwnProfile;

public class GetOwnProfileEndpoint : IEndpoint<Ok<OwnProfileResponse>, GetOwnProfileQuery, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/v1/profile",
                async (ISender sender, CancellationToken ct) =>
                    await HandleAsync(new GetOwnProfileQuery(), sender, ct)
            )
            .WithName("GetOwnProfile")
            .WithTags("Profile")
            .WithSummary("Получить собственный профиль")
            .WithDescription("Возвращает информацию о профиле текущего пользователя")
            .Produces<OwnProfileResponse>()
            .RequireAuthorization();
    }

    public async Task<Ok<OwnProfileResponse>> HandleAsync(
        GetOwnProfileQuery query,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(query, cancellationToken);
        return TypedResults.Ok(result);
    }
}
