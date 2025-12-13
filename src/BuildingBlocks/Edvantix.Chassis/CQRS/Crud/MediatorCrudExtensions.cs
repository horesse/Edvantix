using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.CQRS.Crud.Handlers.Commands;
using Edvantix.Chassis.CQRS.Crud.Handlers.Queries;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Generic;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS.Crud;

public static class MediatorCrudExtensions
{
    public static IServiceCollection AddCrudHandlers<
        TModel,
        TIdentity,
        TEntity,
        TCommonSpecification,
        TPagedSpecification
    >(this IServiceCollection services)
        where TModel : Model<TIdentity>
        where TIdentity : struct
        where TEntity : Entity<TIdentity>, IAggregateRoot
        where TCommonSpecification : ISpecification<TEntity>
        where TPagedSpecification : PagedSpecification<TEntity>
    {
        // Query Handlers
        services.AddScoped<
            IRequestHandler<GetAllByIdsQuery<TModel, TIdentity>, IEnumerable<TModel>>,
            GetAllByIdsQueryHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<GetByIdQuery<TModel, TIdentity>, TModel>,
            GetByIdQueryHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<GetCountQuery, long>,
            GetCountQueryHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<IsExistQuery<TIdentity>, bool>,
            IsExistQueryHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<
                GetByExpressionQuery<TEntity, TModel, TCommonSpecification, TIdentity>,
                IEnumerable<TModel>
            >,
            GetByExpressionQueryHandler<TEntity, TModel, TCommonSpecification, TIdentity>
        >();

        services.AddScoped<
            IRequestHandler<
                FetchPagedDataQuery<TEntity, TModel, TPagedSpecification, TIdentity>,
                PagedResult<TModel>
            >,
            FetchPagedDataQueryHandler<TEntity, TModel, TPagedSpecification, TIdentity>
        >();

        // Command Handlers
        services.AddScoped<
            IRequestHandler<CreateCommand<TModel, TIdentity>, TIdentity>,
            CreateCommandHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<UpdateCommand<TModel, TIdentity>, TIdentity>,
            UpdateCommandHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<DeleteCommand<TIdentity>, TIdentity>,
            DeleteCommandHandler<TModel, TIdentity, TEntity>
        >();

        services.AddScoped<
            IRequestHandler<DeleteRangeCommand<TIdentity>, IEnumerable<TIdentity>>,
            DeleteRangeCommandHandler<TModel, TIdentity, TEntity>
        >();

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
