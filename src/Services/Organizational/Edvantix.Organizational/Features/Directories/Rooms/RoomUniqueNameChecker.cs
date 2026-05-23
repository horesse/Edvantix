using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate;
using Edvantix.Organizational.Domain.AggregatesModel.RoomAggregate.Specifications;
using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Features.Directories.Rooms;

/// <summary>
/// Реализация <see cref="IUniqueNameChecker"/> для справочника «Кабинеты».
/// Делегирует проверку в <see cref="IRoomRepository"/> через спецификацию.
/// </summary>
internal sealed class RoomUniqueNameChecker(IRoomRepository repository) : IUniqueNameChecker
{
    /// <inheritdoc/>
    public string DirectoryCode => DirectoryCatalog.Rooms;

    /// <inheritdoc/>
    public Task<bool> ExistsAsync(
        Guid organizationId,
        string name,
        Guid? excludeId,
        CancellationToken ct
    ) =>
        repository.AnyAsync(
            new RoomUniqueNameSpecification(organizationId, name, excludeId),
            ct
        );
}
