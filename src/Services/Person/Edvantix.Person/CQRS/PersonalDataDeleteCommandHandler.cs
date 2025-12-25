using Edvantix.Chassis.CQRS.Crud.Handlers;
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
    : BaseCrudHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<PersonalDataDeleteCommand<TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
{
    public async Task<TIdentity> Handle(
        PersonalDataDeleteCommand<TIdentity> command,
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

                await Repository.DeleteAsync(command.Id, token);
                await Repository.SaveEntitiesAsync(token);
                return command.Id;
            },
            nameof(PersonalDataDeleteCommand<TIdentity>),
            token
        );
    }
}
