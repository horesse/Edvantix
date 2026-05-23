using Edvantix.Groups.Domain.LessonTypeAggregate;
using Edvantix.Groups.Domain.LessonTypeAggregate.Specifications;

namespace Edvantix.Groups.Features.Directories.LessonTypes;

/// <summary>
/// Проверяет уникальность имени и кода типа занятия в рамках организации
/// среди не архивных записей.
/// </summary>
internal sealed class LessonTypeUniqueChecker(ILessonTypeRepository repository)
    : ILessonTypeUniqueChecker
{
    public Task<bool> NameExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    ) => repository.AnyAsync(new LessonTypeUniqueNameSpec(organizationId, name, excludeId), ct);

    public Task<bool> CodeExistsAsync(
        Guid organizationId,
        string code,
        Guid? excludeId,
        CancellationToken ct
    ) => repository.AnyAsync(new LessonTypeUniqueCodeSpec(organizationId, code, excludeId), ct);
}
