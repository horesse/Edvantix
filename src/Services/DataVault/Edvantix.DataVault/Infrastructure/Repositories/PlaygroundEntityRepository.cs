using Edvantix.Chassis.Repository.Crud;
using Edvantix.DataVault.Domain.AggregatesModel.PlaygroundEntityAggregate;

namespace Edvantix.DataVault.Infrastructure.Repositories;

public sealed class PlaygroundEntityRepository(IServiceProvider provider)
    : SoftDeleteRepository<DataVaultContext, PlaygroundEntity, long>(provider),
        IPlaygroundEntityRepository;
