using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features.GetAdminPost;

public sealed record GetAdminPostQuery(Guid PostId) : IQuery<PostModel>;

public sealed class GetAdminPostQueryHandler(
    IPostRepository postRepository,
    IMapper<Post, PostModel> postMapper,
    IProfileService profileService,
    IMapper<ProfileReply, AuthorModel> authorMapper,
    ClaimsPrincipal claims,
    IPostLikeRepository postLikeRepository
) : IQueryHandler<GetAdminPostQuery, PostModel>
{
    public async ValueTask<PostModel> Handle(
        GetAdminPostQuery request,
        CancellationToken cancellationToken
    )
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

        Guard.Against.NotFound(post, request.PostId);

        var result = postMapper.Map(post);
        await result.EnrichIsLikeByMe(claims, postLikeRepository, cancellationToken);
        await result.EnrichAuthor(post.AuthorId, profileService, authorMapper, cancellationToken);

        return result;
    }
}
