using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Chassis.Endpoints;
using Edvantix.Constants.Core;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Blog.Features.PostFeature.Features.UpdatePost;

/// <summary>
/// Запрос на обновление поста от клиента.
/// </summary>
public sealed record UpdatePostRequest(
    string Title,
    string Slug,
    string Content,
    string? Summary,
    PostType Type,
    bool IsPremium,
    string? CoverImageUrl,
    IReadOnlyList<long> CategoryIds,
    IReadOnlyList<long> TagIds
);

/// <summary>
/// Административный эндпоинт для обновления поста блога.
/// </summary>
public sealed class UpdatePostEndpoint : IEndpoint<NoContent, UpdatePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "/admin/posts/{postId:long}",
                async (
                    long postId,
                    UpdatePostRequest request,
                    ISender sender,
                    CancellationToken ct
                ) =>
                {
                    var command = new UpdatePostCommand(
                        postId,
                        request.Title,
                        request.Slug,
                        request.Content,
                        request.Summary,
                        request.Type,
                        request.IsPremium,
                        request.CoverImageUrl,
                        request.CategoryIds,
                        request.TagIds
                    );

                    return await HandleAsync(command, sender, ct);
                }
            )
            .WithName("UpdatePost")
            .WithTags("Admin.Posts")
            .WithSummary("Обновить пост")
            .WithDescription("Обновляет содержимое поста. Доступно только администраторам.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<NoContent> HandleAsync(
        UpdatePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        await sender.Send(command, cancellationToken);
        return TypedResults.NoContent();
    }
}
