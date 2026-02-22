namespace Edvantix.Blog.Features.TagFeature;

public sealed class Mapper : Mapper<Tag, TagModel>
{
    public override TagModel Map(Tag source)
    {
        return new TagModel()
        {
            Id = source.Id,
            Name = source.Name,
            Slug = source.Slug,
            CreatedAt = source.CreatedAt,
        };
    }
}
