using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.CQRS.Crud.Handlers.Commands;
using Edvantix.Chassis.CQRS.Crud.Handlers.Queries;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Generic;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS.Crud;

public static class MediatorCrudExtensions
{
    /// <summary>
    /// Регистрирует все CRUD handlers для модели
    /// </summary>
    public static IServiceCollection AddCrudHandlers<TModel, TIdentity, TEntity, TSpecification>(
        this IServiceCollection services
    )
        where TModel : Model<TIdentity>
        where TIdentity : struct
        where TEntity : Entity<TIdentity>, IAggregateRoot
        where TSpecification : class, ISpecification<TEntity>, new()
    {
        return services.AddCrudHandlers<TModel, TIdentity, TEntity, TSpecification>(
            CrudActions.All
        );
    }

    /// <summary>
    /// Регистрирует выборочные CRUD handlers для модели
    /// </summary>
    public static IServiceCollection AddCrudHandlers<TModel, TIdentity, TEntity, TSpecification>(
        this IServiceCollection services,
        CrudActions actions
    )
        where TModel : Model<TIdentity>
        where TIdentity : struct
        where TEntity : Entity<TIdentity>, IAggregateRoot
        where TSpecification : class, ISpecification<TEntity>, new()
    {
        // Query Handlers
        if (actions.HasFlag(CrudActions.GetById))
        {
            services.AddScoped<
                IRequestHandler<GetByIdQuery<TModel, TIdentity>, TModel>,
                GetByIdQueryHandler<TModel, TIdentity, TEntity>
            >();
        }

        if (actions.HasFlag(CrudActions.GetCount))
        {
            services.AddScoped<
                IRequestHandler<GetCountQuery, long>,
                GetCountQueryHandler<TModel, TIdentity, TEntity>
            >();
        }

        if (actions.HasFlag(CrudActions.IsExist))
        {
            services.AddScoped<
                IRequestHandler<IsExistQuery<TIdentity>, bool>,
                IsExistQueryHandler<TModel, TIdentity, TEntity>
            >();
        }

        if (actions.HasFlag(CrudActions.GetByExpression))
        {
            services.AddScoped<
                IRequestHandler<
                    GetByExpressionQuery<TEntity, TModel, TSpecification, TIdentity>,
                    IEnumerable<TModel>
                >,
                GetByExpressionQueryHandler<TEntity, TModel, TSpecification, TIdentity>
            >();
        }

        if (actions.HasFlag(CrudActions.FetchPagedData))
        {
            services.AddScoped<
                IRequestHandler<
                    FetchPagedDataQuery<TEntity, TModel, TSpecification, TIdentity>,
                    PagedResult<TModel>
                >,
                FetchPagedDataQueryHandler<TEntity, TModel, TSpecification, TIdentity>
            >();
        }

        // Command Handlers
        if (actions.HasFlag(CrudActions.Create))
        {
            services.AddScoped<
                IRequestHandler<CreateCommand<TModel, TIdentity>, TIdentity>,
                CreateCommandHandler<TModel, TIdentity, TEntity>
            >();
        }

        if (actions.HasFlag(CrudActions.Update))
        {
            services.AddScoped<
                IRequestHandler<UpdateCommand<TModel, TIdentity>, TIdentity>,
                UpdateCommandHandler<TModel, TIdentity, TEntity>
            >();
        }

        if (actions.HasFlag(CrudActions.Delete))
        {
            services.AddScoped<
                IRequestHandler<DeleteCommand<TIdentity>, TIdentity>,
                DeleteCommandHandler<TModel, TIdentity, TEntity>
            >();
        }

        if (actions.HasFlag(CrudActions.DeleteRange))
        {
            services.AddScoped<
                IRequestHandler<DeleteRangeCommand<TIdentity>, IEnumerable<TIdentity>>,
                DeleteRangeCommandHandler<TModel, TIdentity, TEntity>
            >();
        }

        return services;
    }

    public static IServiceCollection AddCrudHandler<THandler, TRequest, TResponse>(
        this IServiceCollection services
    )
        where THandler : class, IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        var descriptor = services.FirstOrDefault(d =>
            d.ServiceType == typeof(IRequestHandler<TRequest, TResponse>)
        );

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
        return services;
    }
}
