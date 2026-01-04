using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

public sealed class UpdateCommandHandler<TModel, TIdentity, TEntity>(IServiceProvider provider)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<UpdateCommand<TModel, TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>
{
    public async Task<TIdentity> Handle(
        UpdateCommand<TModel, TIdentity> command,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entity = await Repository.GetByIdAsync(command.Id, token);
                Guard.Against.NotFound(entity, command.Id);

                Converter.SetProperties(command.Model, entity);

                await Repository.SaveEntitiesAsync(token);
                return entity.Id;
            },
            nameof(UpdateCommand<,>),
            token
        );
    }
}
