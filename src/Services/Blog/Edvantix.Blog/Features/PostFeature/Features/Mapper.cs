using Edvantix.Blog.Features.CategoryFeature;
using Edvantix.Blog.Features.PostFeature.Models;
using Edvantix.Blog.Features.TagFeature;
using Edvantix.Persona.Grpc.Services;

namespace Edvantix.Blog.Features.PostFeature.Features;

public sealed class PostMapper(
    IMapper<Tag, TagModel> tagMapper,
    IMapper<Category, CategoryModel> categoryMapper
) : Mapper<Post, PostModel>
{
    public override PostModel Map(Post source)
    {
        return new PostModel
        {
            Id = source.Id,
            Title = source.Title,
            Slug = source.Slug,
            Content = source.Content,
            Summary = source.Summary,
            Type = source.Type,
            Status = source.Status,
            IsPremium = source.IsPremium,
            CoverImageUrl = source.CoverImageUrl,
            LikesCount = source.LikesCount,
            PublishedAt = source.PublishedAt,
            ScheduledAt = source.ScheduledAt,
            CreatedAt = source.CreatedAt,
            UpdatedAt = source.UpdatedAt,
            Categories = [.. source.Categories.Select(categoryMapper.Map)],
            Tags = [.. source.Tags.Select(tagMapper.Map)],
        };
    }
}

public sealed class PostSummaryMapper(
    IMapper<Tag, TagModel> tagMapper,
    IMapper<Category, CategoryModel> categoryMapper
) : Mapper<Post, PostSummaryModel>
{
    public override PostSummaryModel Map(Post source)
    {
        return new PostSummaryModel
        {
            Id = source.Id,
            Title = source.Title,
            Slug = source.Slug,
            Summary = source.Summary,
            Status = source.Status,
            Type = source.Type,
            IsPremium = source.IsPremium,
            CoverImageUrl = source.CoverImageUrl,
            LikesCount = source.LikesCount,
            PublishedAt = source.PublishedAt,
            ScheduledAt = source.ScheduledAt,
            Categories = [.. source.Categories.Select(categoryMapper.Map)],
            Tags = [.. source.Tags.Select(tagMapper.Map)],
        };
    }
}

public sealed class AuthorMapper : Mapper<ProfileReply, AuthorModel>
{
    public override AuthorModel Map(ProfileReply source)
    {
        return new AuthorModel { Id = Guid.Parse(source.Id), FullName = source.FullName };
    }
}
