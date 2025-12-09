using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class DeleteRangeCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<DeleteRangeCommand<TModel, TIdentity>, IEnumerable<TIdentity>>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async Task<IEnumerable<TIdentity>> Handle(
        DeleteRangeCommand<TModel, TIdentity> command,
        CancellationToken token)
    {
        return await ExecuteAsync(async () =>
        {
            await Repository.DeleteAsync(command.Ids, token);
            await Repository.SaveEntitiesAsync(token);
            return command.Ids;
        }, nameof(DeleteRangeCommand<TModel, TIdentity>), token);
    }
}

