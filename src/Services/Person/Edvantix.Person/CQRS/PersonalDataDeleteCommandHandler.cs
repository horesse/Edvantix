using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Person.Domain.Abstractions;
using MediatR;

namespace Edvantix.Person.CQRS;

/// <summary>
/// Обработчик удаления PersonalData с проверкой владельца (aggregate root подход)
/// </summary>
public sealed class PersonalDataDeleteCommandHandler<TModel, TIdentity, TEntity>(
    IServiceProvider provider
)
    : PersonalDataBaseHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<DeleteCommand<TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
{
    public async Task<TIdentity> Handle(DeleteCommand<TIdentity> command, CancellationToken token)
    {
        return await ExecuteAsync(
            async () =>
            {
                var entity = await Repository.GetByIdAsync(command.Id, token);
                Guard.Against.NotFound(entity, command.Id);

                await SetPersonInfoId(entity, token);

                await Repository.DeleteAsync(command.Id, token);
                await Repository.SaveEntitiesAsync(token);
                return command.Id;
            },
            nameof(DeleteCommand<>),
            token
        );
    }
}
