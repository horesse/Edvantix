namespace Edvantix.Blog.Features.PostFeature.Features.CreatePost;

public sealed class CreatePostEndpoint : IEndpoint<Created<Guid>, CreatePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/posts",
                async (
                    CreatePostCommand request,
                    ISender sender,
                    CancellationToken cancellationToken
                ) => await HandleAsync(request, sender, cancellationToken)
            )
            .WithName("Создать пост")
            .WithTags("Администрирование")
            .WithSummary("Создать пост")
            .WithDescription(
                "Создаёт новый черновик поста блога. Доступно только администраторам платформы."
            )
            .Produces<Guid>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Created<Guid>> HandleAsync(
        CreatePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/posts/{id}", id);
    }
}
