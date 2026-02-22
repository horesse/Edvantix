using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features;

public static class Enrich
{
    extension(PostModel post)
    {
        public async Task EnrichAuthor(
            ulong authorId,
            IServiceProvider provider,
            CancellationToken cancellationToken
        )
        {
            var profileService = provider.GetRequiredService<IProfileService>();
            var authorProfile = await profileService.GetProfileById(authorId, cancellationToken);

            if (authorProfile is null)
            {
                post.Author = new AuthorModel { Id = 0, FullName = "Анонимно" };
                return;
            }

            var authorMapper = provider.GetRequiredService<IMapper<ProfileReply, AuthorModel>>();

            post.Author = authorMapper.Map(authorProfile);
        }

        public async Task EnrichIsLikeByMe(
            IServiceProvider provider,
            CancellationToken cancellationToken
        )
        {
            var currentUserId = await provider.TryGetProfileId(cancellationToken);
            post.IsLikedByMe = false;

            if (currentUserId.HasValue)
            {
                var likeRepo = provider.GetRequiredService<IPostLikeRepository>();
                var likeSpec = new PostLikeSpecification(
                    postId: post.Id,
                    userId: currentUserId.Value
                );
                var existingLike = await likeRepo.Get(likeSpec, cancellationToken);
                post.IsLikedByMe = existingLike is not null;
            }
        }
    }
}
