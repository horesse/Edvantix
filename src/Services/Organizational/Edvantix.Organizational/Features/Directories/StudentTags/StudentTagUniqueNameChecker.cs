using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentTags;

/// <summary>Проверка уникальности имени тега студента в рамках организации.</summary>
internal sealed class StudentTagUniqueNameChecker(IStudentTagRepository repository)
    : IUniqueNameChecker
{
    /// <inheritdoc/>
    public string DirectoryCode => DirectoryCatalog.Tags;

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    ) =>
        repository.AnyAsync(
            new StudentTagUniqueNameSpecification(organizationId, name, excludeId),
            ct
        );
}
