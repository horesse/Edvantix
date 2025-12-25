using Edvantix.Chassis.CQRS.Crud;
using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.Specification;
using Edvantix.Constants.Other;
using Edvantix.Person.CQRS;
using Edvantix.Person.Domain.Abstractions;
using Edvantix.Person.Features;
using Edvantix.SharedKernel.SeedWork;
using MediatR;

namespace Edvantix.Person.CQRS.Extensions;

public static class PersonalDataExtensions
{
    /// <summary>
    /// Регистрирует PersonalData CRUD handlers с автоматическим заполнением PersonInfoId.
    /// Использует aggregate root подход с проверкой владельца для Update/Delete операций.
    /// </summary>
    public static IServiceCollection AddPersonalDataCrudHandlers<
        TModel,
        TIdentity,
        TEntity,
        TSpecification
    >(this IServiceCollection services, CrudActions actions = CrudActions.All)
        where TModel : class
        where TIdentity : struct
        where TEntity : PersonalData<TIdentity>, IAggregateRoot
        where TSpecification : class, ISpecification<TEntity>, new()
    {
        // Регистрируем стандартные query handlers (без изменений)
        var queryActions =
            actions
            & (
                CrudActions.GetById
                | CrudActions.GetCount
                | CrudActions.IsExist
                | CrudActions.GetByExpression
                | CrudActions.FetchPagedData
            );

        if (queryActions != CrudActions.None)
        {
            services.AddCrudHandlers<TModel, TIdentity, TEntity, TSpecification>(queryActions);
        }

        // Command Handlers для PersonalData с проверками владельца
        if (actions.HasFlag(CrudActions.Create))
        {
            services.AddCrudHandler<
                PersonalDataCreateCommandHandler<TModel, TIdentity, TEntity>,
                CreateCommand<TModel, TIdentity>,
                TIdentity
            >();
        }

        if (actions.HasFlag(CrudActions.Update))
        {
            services.AddCrudHandler<
                PersonalDataUpdateCommandHandler<TModel, TIdentity, TEntity>,
                UpdateCommand<TModel, TIdentity>,
                TIdentity
            >();
        }

        if (actions.HasFlag(CrudActions.Delete))
        {
            services.AddCrudHandler<
                PersonalDataDeleteCommandHandler<TModel, TIdentity, TEntity>,
                DeleteCommand<TIdentity>,
                TIdentity
            >();
        }

        return services;
    }
}
