using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Edvantix.Persona.Features.Profiles.Create;

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
                    CancellationToken cancellationToken
                ) => await HandleAsync(command, sender, linker, cancellationToken)
            )
            .Accepts<RegistrationCommand>(MediaTypeNames.Multipart.FormData)
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status422UnprocessableEntity)
            .WithName("Регистрация профиля")
            .WithTags("Профиль")
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
