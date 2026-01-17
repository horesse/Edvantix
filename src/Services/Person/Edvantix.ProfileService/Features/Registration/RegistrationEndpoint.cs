using Edvantix.Chassis.Endpoints;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.ProfileService.Features.Registration;

public class RegistrationEndpoint : IEndpoint<Created<long>, RegistrationCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/api/person/registration",
                async (RegistrationCommand command, ISender sender, CancellationToken ct) =>
                    await HandleAsync(command, sender, ct)
            )
            .WithName("Registration")
            .WithTags("Person")
            .WithSummary("Регистрация пользователя")
            .WithDescription("Создать новую запись пользователя")
            .Produces<long>(StatusCodes.Status201Created);
    }

    public async Task<Created<long>> HandleAsync(
        RegistrationCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/person/{id}", id);
    }
}
