using Edvantix.Chassis.CQRS.Crud.Handlers;
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
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<PersonalDataUpdateCommand<TModel, TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
{
    public async Task<TIdentity> Handle(
        PersonalDataUpdateCommand<TModel, TIdentity> command,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entity = await Repository.GetByIdAsync(command.Id, token);
                Guard.Against.NotFound(entity, command.Id);

                // Проверка принадлежности к пользователю (aggregate root boundary)
                if (entity.PersonInfoId != command.PersonInfoId)
                {
                    throw new Exception(
                        $"User does not have access to PersonalData with Id={command.Id}"
                    );
                }

                Converter.SetProperties(command.Model, entity);

                await Repository.SaveEntitiesAsync(token);
                return entity.Id;
            },
            nameof(PersonalDataUpdateCommand<,>),
            token
        );
    }
}
