using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using Mediator;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class UpdateCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        ICommandHandler<UpdateCommand<TModel, TIdentity>, TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async ValueTask<TIdentity> Handle(
        UpdateCommand<TModel, TIdentity> command,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            var entity = ModelToEntityMapper.Map(command.Model);
            var modified = await Repository.UpdateAsync(entity, token);
            return modified.Id;
        }, nameof(UpdateCommand<,>), token);
    }
}
