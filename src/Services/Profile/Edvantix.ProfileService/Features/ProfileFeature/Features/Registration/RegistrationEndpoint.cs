using System.Net.Mime;
using Edvantix.Chassis.Endpoints;
using Edvantix.ServiceDefaults.Kestrel;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.ProfileService.Features.ProfileFeature.Registration;

public class RegistrationEndpoint
    : IEndpoint<Created<long>, RegistrationCommand, ISender, LinkGenerator>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/profile/registration",
                async (
                    [FromForm] RegistrationCommand command,
                    ISender sender,
                    LinkGenerator linker,
                    CancellationToken ct
                ) => await HandleAsync(command, sender, linker, ct)
            )
            .Accepts<RegistrationCommand>(MediaTypeNames.Multipart.FormData)
            .Produces<long>(StatusCodes.Status201Created)
            .WithName("Registration")
            .WithTags("Profile")
            .WithSummary("Регистрация пользователя")
            .WithDescription("Создать новую запись пользователя")
            .WithFormOptions(true)
            .MapToApiVersion(new(1, 0))
            .RequirePerUserRateLimit();
    }

    public async Task<Created<long>> HandleAsync(
        RegistrationCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);

        // TODO: Get Profile by ID
        // var path = linker.GetPathByName(nameof(GetProfileById), new { id = result });

        return TypedResults.Created($"/api/profile/{id}", id);
    }
}
