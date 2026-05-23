using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses;

/// <summary>
/// Реализация <see cref="IUniqueNameChecker"/> для справочника «Статусы студентов».
/// Делегирует проверку в <see cref="IStudentStatusRepository"/> через спецификацию.
/// </summary>
internal sealed class StudentStatusUniqueNameChecker(IStudentStatusRepository repository)
    : IUniqueNameChecker
{
    /// <inheritdoc/>
    public string DirectoryCode => DirectoryCatalog.StudentStatuses;

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    ) =>
        repository.AnyAsync(
            new StudentStatusUniqueNameSpecification(organizationId, name, excludeId),
            ct
        );
}
