namespace Edvantix.Blog.Features.TagFeature.CreateTag;

public sealed class CreateTagEndpoint : IEndpoint<Created<Guid>, CreateTagCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/tags",
                async (
                    CreateTagCommand request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("Создать тег")
            .WithTags("Администрирование")
            .WithSummary("Создать тег")
            .WithDescription("Создаёт новый тег блога. Доступно только администраторам.")
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Created<Guid>> HandleAsync(
        CreateTagCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/tags/{id}", id);
    }
}
