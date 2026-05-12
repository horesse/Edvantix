using Edvantix.Curriculum.Domain.Enums;

namespace Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate.Specifications;

/// <summary>Активные курсы организации — лёгкое представление для выпадающих списков и gRPC.</summary>
internal sealed class CourseOptionsSpecification : Specification<Course>
{
    public CourseOptionsSpecification(Guid organizationId, string? search)
    {
        Query
            .AsNoTracking()
            .Where(c => c.OrganizationId == organizationId && c.Status == CourseStatus.Active)
            .OrderBy(c => c.Code);

        if (!string.IsNullOrWhiteSpace(search))
        {
            Query.Search(c => c.Name, $"%{search}%", 1);
            Query.Search(c => c.Code, $"%{search.ToUpperInvariant()}%", 1);
        }
    }
}
