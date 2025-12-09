using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class UpdateRangeCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<UpdateRangeCommand<TModel, TIdentity>, IEnumerable<TIdentity>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async Task<IEnumerable<TIdentity>> Handle(
        UpdateRangeCommand<TModel, TIdentity> command,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entities = command.Models.Select(ModelToEntityMapper.Map).ToList();
            var modified = await Repository.UpdateAsync(entities, token);
            await Repository.SaveEntitiesAsync(token);
            return modified.Select(c => c.Id);
        }, nameof(UpdateRangeCommand<TModel, TIdentity>), token);
    }
}
