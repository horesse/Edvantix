using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS.Crud.Handlers.Commands;

/// <summary>
/// Обработчик создания записи с использованием CreateViewModel
/// </summary>
public sealed class CreateWithViewModelCommandHandler<TModel, TCreateViewModel, TIdentity, TEntity>(
    IServiceProvider provider
)
    : BaseCrudViewModelHandler<TModel, TIdentity, TEntity>(provider),
        IRequestHandler<CreateWithViewModelCommand<TModel, TCreateViewModel, TIdentity>, TIdentity>
    where TModel : Model<TIdentity>
    where TCreateViewModel : class
    where TIdentity : struct
    where TEntity : Entity<TIdentity>, IAggregateRoot
{
    private readonly Func<TCreateViewModel, TModel> _viewModelToModelMapper =
        provider.GetRequiredService<Func<TCreateViewModel, TModel>>();

    public async Task<TIdentity> Handle(
        CreateWithViewModelCommand<TModel, TCreateViewModel, TIdentity> command,
        CancellationToken token
    )
    {
        return await ExecuteAsync(
            async () =>
            {
                // Маппинг CreateViewModel -> Model
                var model = _viewModelToModelMapper(command.ViewModel);

                // Маппинг Model -> Entity
                var entity = Converter.Map(model);

                var added = await Repository.InsertAsync(entity, token);
                await Repository.SaveEntitiesAsync(token);
                return added.Id;
            },
            nameof(CreateWithViewModelCommand<,,>),
            token
        );
    }
}
