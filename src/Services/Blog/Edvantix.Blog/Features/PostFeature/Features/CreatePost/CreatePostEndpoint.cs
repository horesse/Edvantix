using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Chassis.Endpoints;
using Edvantix.Constants.Core;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Edvantix.Blog.Features.PostFeature.Features.CreatePost;

/// <summary>
/// Запрос на создание нового поста от клиента.
/// </summary>
public sealed record CreatePostRequest(
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
/// Административный эндпоинт для создания нового поста блога.
/// Доступен только пользователям с ролью администратора.
/// </summary>
public sealed class CreatePostEndpoint : IEndpoint<Created<long>, CreatePostCommand, ISender>
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(
                "/admin/posts",
                async (CreatePostRequest request, ISender sender, CancellationToken ct) =>
                {
                    var command = new CreatePostCommand(
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
            .WithName("CreatePost")
            .WithTags("Admin.Posts")
            .WithSummary("Создать пост")
            .WithDescription(
                "Создаёт новый черновик поста блога. Доступно только администраторам платформы."
            )
            .Produces<long>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .RequireAuthorization(Authorization.Policies.Admin);
    }

    public async Task<Created<long>> HandleAsync(
        CreatePostCommand command,
        ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var id = await sender.Send(command, cancellationToken);
        return TypedResults.Created($"/api/v1/posts/{id}", id);
    }
}
