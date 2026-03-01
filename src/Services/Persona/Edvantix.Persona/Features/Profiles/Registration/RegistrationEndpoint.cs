using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Persona.Features.Profiles.Registration;

/// <summary>POST /v1/profile/registration — первичная регистрация профиля.</summary>
public sealed class RegistrationEndpoint
    : IEndpoint<Created<Guid>, RegistrationCommand, ISender, LinkGenerator>
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
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("Registration")
            .WithTags("Profile")
            .WithSummary("Регистрация пользователя")
            .WithDescription("Создаёт новый профиль для аутентифицированного пользователя.")
            .WithFormOptions(true)
            .MapToApiVersion(ApiVersions.V1)
            .RequirePerUserRateLimit()
            .RequireAuthorization();
    }

    public async Task<Created<Guid>> HandleAsync(
        RegistrationCommand command,
        ISender sender,
        LinkGenerator linker,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        var location = linker.GetPathByName("GetProfileById", new { id }) ?? $"/api/profiles/{id}";
        return TypedResults.Created(location, id);
    }
}
