using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Utilities;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features;

public static class Enrich
{
    private static async Task<AuthorModel> ResolveAuthor(
        Guid authorId,
        IProfileService profileService,
        IMapper<ProfileReply, AuthorModel> authorMapper,
        CancellationToken cancellationToken
    )
    {
        var authorProfile = await profileService.GetProfileById(authorId, cancellationToken);

        if (authorProfile is null)
            return new AuthorModel { Id = Guid.Empty, FullName = "Анонимно" };

        return authorMapper.Map(authorProfile);
    }

    extension(PostModel post)
    {
        public async Task EnrichAuthor(
            Guid authorId,
            IProfileService profileService,
            IMapper<ProfileReply, AuthorModel> authorMapper,
            CancellationToken cancellationToken
        )
        {
            post.Author = await ResolveAuthor(authorId, profileService, authorMapper, cancellationToken);
        }

        public async Task EnrichIsLikeByMe(
            ClaimsPrincipal claims,
            IPostLikeRepository likeRepository,
            CancellationToken cancellationToken
        )
        {
            var currentUserId = claims.TryGetProfileId();
            post.IsLikedByMe = false;

            if (currentUserId.HasValue)
            {
                var likeSpec = new PostLikeSpecification(
                    postId: post.Id,
                    userId: currentUserId.Value
                );
                var existingLike = await likeRepository.Get(likeSpec, cancellationToken);
                post.IsLikedByMe = existingLike is not null;
            }
        }
    }

    extension(PostSummaryModel post)
    {
        public async Task EnrichAuthor(
            Guid authorId,
            IProfileService profileService,
            IMapper<ProfileReply, AuthorModel> authorMapper,
            CancellationToken cancellationToken
        )
        {
            post.Author = await ResolveAuthor(authorId, profileService, authorMapper, cancellationToken);
        }
    }
}
