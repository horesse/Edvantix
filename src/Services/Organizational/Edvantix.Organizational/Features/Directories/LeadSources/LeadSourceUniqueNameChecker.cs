using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.LeadSources;

/// <summary>
/// Реализация <see cref="IUniqueNameChecker"/> для справочника «Источники привлечения».
/// Делегирует проверку в <see cref="ILeadSourceRepository"/> через спецификацию.
/// </summary>
internal sealed class LeadSourceUniqueNameChecker(ILeadSourceRepository repository)
    : IUniqueNameChecker
{
    /// <inheritdoc/>
    public string DirectoryCode => DirectoryCatalog.Sources;

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    ) =>
        repository.AnyAsync(
            new LeadSourceUniqueNameSpecification(organizationId, name, excludeId),
            ct
        );
}
