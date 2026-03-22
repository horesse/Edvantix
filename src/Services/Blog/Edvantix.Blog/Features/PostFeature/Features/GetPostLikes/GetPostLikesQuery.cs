using Edvantix.Chassis.Utilities.Guards;

namespace Edvantix.Blog.Features.PostFeature.Features.GetPostLikes;

public sealed record GetPostLikesQuery(Guid PostId) : IQuery<PostLikesModel>;

public sealed record PostLikesModel(Guid PostId, int LikesCount);

public sealed class GetPostLikesQueryHandler(IPostRepository postRepository)
    : IQueryHandler<GetPostLikesQuery, PostLikesModel>
{
    public async ValueTask<PostLikesModel> Handle(
        GetPostLikesQuery request,
        CancellationToken cancellationToken
    )
    {
        var post = await postRepository.GetByIdAsync(request.PostId, cancellationToken);

        Guard.Against.NotFound(post, request.PostId);

        return new PostLikesModel(post.Id, post.LikesCount);
    }
}
