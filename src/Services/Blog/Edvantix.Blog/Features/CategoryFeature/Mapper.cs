namespace Edvantix.Blog.Features.CategoryFeature;

public sealed class Mapper : Mapper<Category, CategoryModel>
{
    public override CategoryModel Map(Category source)
    {
        return new CategoryModel
        {
            Id = source.Id,
            Name = source.Name,
            Slug = source.Slug,
            Description = source.Description,
            CreatedAt = source.CreatedAt,
        };
    }
}
