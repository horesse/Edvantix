namespace Edvantix.Blog.Features.CategoryFeature.DeleteCategory;

/// <summary>
/// Административный эндпоинт для удаления категории блога.
/// </summary>
public sealed class DeleteCategoryEndpoint : IEndpoint<NoContent, DeleteCategoryCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "/admin/categories/{categoryId:guid}",
                async (Guid categoryId, ISender sender, CancellationToken cancellationToken) =>
                    await HandleAsync(
                        new DeleteCategoryCommand(categoryId),
                        sender,
                        cancellationToken
                    )
            )
            .WithName("DeleteCategory")
            .WithTags("Admin.Categories")
            .WithSummary("Удалить категорию")
            .WithDescription("Удаляет категорию блога. Доступно только администраторам.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        DeleteCategoryCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
