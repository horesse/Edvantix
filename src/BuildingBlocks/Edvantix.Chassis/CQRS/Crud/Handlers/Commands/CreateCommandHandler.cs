using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class CreateCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        ICommandHandler<CreateCommand<TModel, TIdentity>, TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async ValueTask<TIdentity> Handle(
        CreateCommand<TModel, TIdentity> command,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entity = ModelToEntityMapper.Map(command.Model);
            var added = await Repository.InsertAsync(entity, token);
            return added.Id;
        }, nameof(CreateCommand<,>), token);
    }
}
