using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Utilities.Guards;
using Edvantix.Person.Domain.Abstractions;
using MediatR;

namespace Edvantix.Person.CQRS;

/// <summary>
/// Обработчик обновления PersonalData с проверкой владельца (aggregate root подход)
/// </summary>
public sealed class PersonalDataUpdateCommandHandler<TModel, TIdentity, TEntity>(
    IServiceProvider provider
)
    : PersonalDataBaseHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<UpdateCommand<TModel, TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
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

                await SetPersonInfoId(entity, token);

                Converter.SetProperties(command.Model, entity);

                await Repository.SaveEntitiesAsync(token);
                return entity.Id;
            },
            nameof(UpdateCommand<,>),
            token
        );
    }
}
