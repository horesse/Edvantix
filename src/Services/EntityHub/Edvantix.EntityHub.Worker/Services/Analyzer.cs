using System.Reflection;
using System.Runtime.CompilerServices;
using Edvantix.Chassis.Utilities.Attributes;
using Edvantix.Constants.Other;
using Edvantix.EntityHub.Domain.AggregatesModel.EntityGroupAggregate;
using Edvantix.EntityHub.Domain.AggregatesModel.EntityTypeAggregate;
using Edvantix.EntityHub.Domain.AggregatesModel.MicroserviceAggregate;

namespace Edvantix.EntityHub.Worker.Services;

file record struct EntityTypeDto(string Name, string Description, EntityGroupEnum Type);

public sealed class Analyzer(IServiceProvider provider)
{
    public async Task AnalyzeAssemblies(CancellationToken token)
    {
        var assemblies = ProjectAssembly.Assemblies;

        var microserviceMap = new Dictionary<string, long>();
        await foreach (var (name, id) in SyncMicroservicesAsync(assemblies, token))
        {
            microserviceMap[name] = id;
        }

        await SyncEntityGroupAsync();

        await Parallel.ForEachAsync(
            assemblies,
            new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
                CancellationToken = token,
            },
            async (kvp, ct) =>
            {
                var (serviceName, assembly) = kvp;
                var microserviceId = microserviceMap[serviceName];
                await SyncEntityTypesAsync(microserviceId, assembly, ct);
            }
        );
    }

    private async IAsyncEnumerable<(string serviceName, long id)> SyncMicroservicesAsync(
        Dictionary<string, Assembly> assemblies,
        [EnumeratorCancellation] CancellationToken token
    )
    {
        var localProvider = provider.CreateScope().ServiceProvider;
        using var repo = localProvider.GetRequiredService<IMicroserviceRepository>();

        var existing = await repo.GetAllAsync(token);
        var existingDict = existing.ToDictionary(x => x.Name, x => x.Id);

        var serviceNamesSet = assemblies.Select(x => x.Key).ToHashSet();

        var toDelete = existing.Where(m => !serviceNamesSet.Contains(m.Name)).ToList();

        if (toDelete.Count > 0)
        {
            await repo.DeleteAsync(toDelete, token);
        }

        foreach (var serviceName in serviceNamesSet)
        {
            if (existingDict.TryGetValue(serviceName, out var id))
            {
                yield return (serviceName, id);
            }
            else
            {
                var newMicroservice = await repo.InsertAsync(
                    new Microservice { Name = serviceName },
                    token
                );
                yield return (serviceName, newMicroservice.Id);
            }
        }

        await repo.SaveEntitiesAsync(token);
    }

    private async Task SyncEntityGroupAsync()
    {
        using var repo = provider.GetRequiredService<IEntityGroupRepository>();
        
        var types = Enum.GetValues<EntityGroupEnum>();

        foreach (var type in types)
        {
            if (await repo.AnyAsync(x => x.Name ==  type.ToString(), CancellationToken.None))
                continue;

            await repo.InsertAsync(new EntityGroup(type.ToString()), CancellationToken.None);
        }

        await repo.SaveEntitiesAsync(CancellationToken.None);
    }
    
    private async Task SyncEntityTypesAsync(
        long microserviceId,
        Assembly assembly,
        CancellationToken token
    )
    {
        var localProvider = provider.CreateScope().ServiceProvider;
        using var repo = localProvider.GetRequiredService<IEntityTypeRepository>();

        var publicModels = assembly
            .GetTypes()
            .Select(t => (Type: t, Attribute: t.GetCustomAttribute<PublicModelAttribute>()))
            .Where(x => x.Attribute is not null)
            .Select(x => new EntityTypeDto(
                Name: x.Type.Name.Replace("Model", "", StringComparison.Ordinal),
                Description: x.Attribute!.Description,
                Type: x.Attribute!.EntityType
            ))
            .ToArray();

        var expression = new EntityTypeFilterExpression(microserviceId);
        var existing = await repo.GetByExpressionAsync(expression, token);

        var existingDict = existing.ToDictionary(e => e.Name, e => e);
        var publicModelTypes = publicModels.Select(p => p.Name).ToHashSet();

        var toDelete = existing.Where(e => !publicModelTypes.Contains(e.Name)).ToList();

        if (toDelete.Count > 0)
        {
            await repo.DeleteAsync(toDelete, token);
        }

        var toInsert = new List<EntityType>();
        var toUpdate = new List<EntityType>();

        foreach (var model in publicModels)
        {
            if (existingDict.TryGetValue(model.Name, out var existingEntity))
            {
                if (
                    existingEntity.Name == model.Name
                    && existingEntity.Description == model.Description
                )
                    continue;

                existingEntity.Update(model.Name, model.Description, (long)model.Type);
                toUpdate.Add(existingEntity);
            }
            else
            {
                toInsert.Add(
                    new EntityType(model.Name, model.Description, microserviceId, (long)model.Type)
                );
            }
        }

        if (toInsert.Count > 0)
        {
            await repo.InsertRangeAsync(toInsert, token);
        }

        if (toUpdate.Count > 0)
        {
            await repo.UpdateAsync(toUpdate, token);
        }

        await repo.SaveEntitiesAsync(token);
    }
}
