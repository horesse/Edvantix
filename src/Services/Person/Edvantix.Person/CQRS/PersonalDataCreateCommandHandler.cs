using Edvantix.Chassis.CQRS.Crud.Handlers;
using Edvantix.Person.Domain.Abstractions;
using MediatR;

namespace Edvantix.Person.CQRS;

/// <summary>
/// Обработчик создания PersonalData с автоматическим заполнением PersonInfoId через behavior
/// </summary>
public sealed class PersonalDataCreateCommandHandler<TModel, TIdentity, TEntity>(
    IServiceProvider provider
)
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<PersonalDataCreateCommand<TModel, TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
{
    public async Task<TIdentity> Handle(
        PersonalDataCreateCommand<TModel, TIdentity> command,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entity = Converter.Map(command.Model);

                // Устанавливаем PersonInfoId (заполняется через PersonalDataBehavior)
                var personInfoIdProperty = typeof(TEntity).GetProperty(
                    nameof(PersonalData<>.PersonInfoId)
                );
                personInfoIdProperty?.SetValue(entity, command.PersonInfoId);

                var added = await Repository.InsertAsync(entity, token);
                await Repository.SaveEntitiesAsync(token);
                return added.Id;
            },
            nameof(PersonalDataCreateCommand<,>),
            token
        );
    }
}
