using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.ProfileService.Domain.Abstractions;
using MediatR;

namespace Edvantix.ProfileService.CQRS;

/// <summary>
/// Обработчик создания PersonalData с автоматическим заполнением PersonInfoId через behavior
/// </summary>
public sealed class PersonalDataCreateCommandHandler<TModel, TIdentity, TEntity>(
    IServiceProvider provider
)
    : PersonalDataBaseHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<CreateCommand<TModel, TIdentity>, TIdentity>
    where TModel : class
    where TIdentity : struct
    where TEntity : PersonalData<TIdentity>
{
    public async Task<TIdentity> Handle(
        CreateCommand<TModel, TIdentity> command,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                var entity = Converter.Map(command.Model);

                await SetPersonInfoId(entity, token);

                var added = await Repository.InsertAsync(entity, token);
                await Repository.SaveEntitiesAsync(token);
                return added.Id;
            },
            nameof(CreateCommand<,>),
            token
        );
    }
}
