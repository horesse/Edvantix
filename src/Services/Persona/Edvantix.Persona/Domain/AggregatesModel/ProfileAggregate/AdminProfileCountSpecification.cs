namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Спецификация для подсчёта профилей в административной панели (без пагинации).
/// </summary>
public sealed class AdminProfileCountSpecification : Specification<Profile>
{
    public AdminProfileCountSpecification(string? search = null, bool? isBlocked = null)
    {
        Query.AsNoTracking();
        AdminProfileListSpecification.ApplyFilters(Query, search, isBlocked);
    }
}
