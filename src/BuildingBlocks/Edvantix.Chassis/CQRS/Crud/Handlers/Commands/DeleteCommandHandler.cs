using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class DeleteCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<DeleteCommand<TIdentity>, TIdentity>
    where TModel : Model<TIdentity>
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    public async Task<TIdentity> Handle(DeleteCommand<TIdentity> command, CancellationToken token)
    {
        return await ExecuteAsync(
            async () =>
            {
                await Repository.DeleteAsync(command.Id, token);
                await Repository.SaveEntitiesAsync(token);
                return command.Id;
            },
            nameof(DeleteCommand<TIdentity>),
            token
        );
    }
}
