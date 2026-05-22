using Edvantix.Organizational.Domain.AggregatesModel.StudentStatusAggregate;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.StudentStatuses;

/// <summary>
/// Реализация <see cref="IUniqueNameChecker"/> для справочника «Статусы студентов».
/// Делегирует проверку в <see cref="IStudentStatusRepository"/>.
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
    ) => repository.ExistsNameAsync(organizationId, name, excludeId, ct);
}
