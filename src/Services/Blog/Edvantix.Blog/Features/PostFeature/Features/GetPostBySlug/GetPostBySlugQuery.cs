using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Utilities;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPostBySlug;

public sealed record GetPostBySlugQuery(string Slug) : IQuery<PostModel>;

public sealed class GetPostBySlugQueryHandler(
    IPostRepository postRepository,
    IMapper<Post, PostModel> postMapper,
    IProfileService profileService,
    IMapper<ProfileReply, AuthorModel> authorMapper,
    ClaimsPrincipal claims,
    IPostLikeRepository postLikeRepository
) : IQueryHandler<GetPostBySlugQuery, PostModel>
{
    public async ValueTask<PostModel> Handle(
        GetPostBySlugQuery request,
        CancellationToken cancellationToken
    )
    {
        var post =
            await postRepository.GetBySlugAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException($"Пост со slug '{request.Slug}' не найден.");

        if (post.Status != PostStatus.Published)
            throw new NotFoundException($"Пост со slug '{request.Slug}' не найден.");

        var result = postMapper.Map(post);
        await result.EnrichIsLikeByMe(claims, postLikeRepository, cancellationToken);
        await result.EnrichAuthor(post.AuthorId, profileService, authorMapper, cancellationToken);

        return result;
    }
}
