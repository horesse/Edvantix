using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class CreateRangeCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<CreateRangeCommand<TModel, TIdentity>, IEnumerable<TIdentity>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async Task<IEnumerable<TIdentity>> Handle(
        CreateRangeCommand<TModel, TIdentity> command,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entities = command.Models.Select(ModelToEntityMapper.Map).ToList();
            var added = await Repository.InsertRangeAsync(entities, token);
            await Repository.SaveEntitiesAsync(token);
            return added.Select(c => c.Id);
        }, nameof(CreateRangeCommand<TModel, TIdentity>), token);
    }
}
