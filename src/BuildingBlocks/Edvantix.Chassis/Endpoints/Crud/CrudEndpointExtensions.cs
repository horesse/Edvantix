using Edvantix.Chassis.Endpoints.Crud.Commands;
using Edvantix.Chassis.Endpoints.Crud.Queries;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Generic;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Endpoints.Crud;

public static class CrudEndpointExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует все стандартные CRUD endpoints для модели
        /// </summary>
        public IServiceCollection AddCrudEndpoints<TEntity, TModel, TIdentity, TSpecification>()
            where TModel : Model<TIdentity>
            where TIdentity : struct
            where TEntity : class, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            return services.AddCrudEndpoints<TEntity, TModel, TIdentity, TSpecification>(
                CrudActions.All
            );
        }

        /// <summary>
        /// Регистрирует выборочные CRUD endpoints для модели
        /// </summary>
        public IServiceCollection AddCrudEndpoints<TEntity, TModel, TIdentity, TSpecification>(
            CrudActions actions
        )
            where TModel : Model<TIdentity>
            where TIdentity : struct
            where TEntity : class, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            // Query Endpoints
            if (actions.HasFlag(CrudActions.GetById))
            {
                services.AddTransient<IEndpoint, GetByIdEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.GetCount))
            {
                services.AddTransient<IEndpoint, GetCountEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.IsExist))
            {
                services.AddTransient<IEndpoint, IsExistEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.GetByExpression))
            {
                services.AddTransient<
                    IEndpoint,
                    GetByExpressionEndpoint<TModel, TIdentity, TEntity, TSpecification>
                >();
            }

            if (actions.HasFlag(CrudActions.FetchPagedData))
            {
                services.AddTransient<
                    IEndpoint,
                    FetchPagedDataEndpoint<TModel, TIdentity, TEntity, TSpecification>
                >();
            }

            // Command Endpoints
            if (actions.HasFlag(CrudActions.Create))
            {
                services.AddTransient<IEndpoint, CreateEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.Update))
            {
                services.AddTransient<IEndpoint, UpdateEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.Delete))
            {
                services.AddTransient<IEndpoint, DeleteEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.DeleteRange))
            {
                services.AddTransient<IEndpoint, DeleteRangeEndpoint<TModel, TIdentity>>();
            }

            return services;
        }

        /// <summary>
        /// Регистрирует CRUD endpoints с поддержкой ViewModels
        /// </summary>
        public IServiceCollection AddCrudViewModelEndpoints<
            TEntity,
            TModel,
            TCreateViewModel,
            TViewViewModel,
            TIdentity,
            TSpecification
        >(
            Func<TCreateViewModel, TModel> createViewModelMapper,
            Func<TModel, TViewViewModel> viewViewModelMapper
        )
            where TModel : Model<TIdentity>
            where TCreateViewModel : class
            where TViewViewModel : class
            where TIdentity : struct
            where TEntity : class, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            return services.AddCrudViewModelEndpoints<
                TEntity,
                TModel,
                TCreateViewModel,
                TViewViewModel,
                TIdentity,
                TSpecification
            >(
                CrudActions.Create | CrudActions.GetById | CrudActions.FetchPagedData,
                createViewModelMapper,
                viewViewModelMapper
            );
        }

        /// <summary>
        /// Регистрирует выборочные CRUD endpoints с поддержкой ViewModels
        /// </summary>
        public IServiceCollection AddCrudViewModelEndpoints<
            TEntity,
            TModel,
            TCreateViewModel,
            TViewViewModel,
            TIdentity,
            TSpecification
        >(
            CrudActions actions,
            Func<TCreateViewModel, TModel> createViewModelMapper,
            Func<TModel, TViewViewModel> viewViewModelMapper
        )
            where TModel : Model<TIdentity>
            where TCreateViewModel : class
            where TViewViewModel : class
            where TIdentity : struct
            where TEntity : class, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            // Регистрируем мапперы
            services.AddSingleton(createViewModelMapper);
            services.AddSingleton(viewViewModelMapper);

            // Command Endpoints
            if (actions.HasFlag(CrudActions.Create))
            {
                services.AddTransient<
                    IEndpoint,
                    CreateWithViewModelEndpoint<TModel, TCreateViewModel, TIdentity>
                >();
            }

            // Query Endpoints
            if (actions.HasFlag(CrudActions.GetById))
            {
                services.AddTransient<IEndpoint, GetByIdWithViewModelEndpoint<TModel, TIdentity>>();
            }

            if (actions.HasFlag(CrudActions.FetchPagedData))
            {
                services.AddTransient<
                    IEndpoint,
                    FetchPagedDataWithViewModelEndpoint<
                        TModel,
                        TViewViewModel,
                        TIdentity,
                        TEntity,
                        TSpecification
                    >
                >();
            }

            return services;
        }

        /// <summary>
        /// Регистрирует кастомный endpoint, переопределяя стандартный
        /// </summary>
        public IServiceCollection AddCrudEndpoint<TEndpoint>()
            where TEndpoint : class, IEndpoint
        {
            // Удаляем существующую регистрацию если есть
            var descriptor = services.FirstOrDefault(d =>
                d.ServiceType == typeof(IEndpoint) && d.ImplementationType == typeof(TEndpoint)
            );

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddTransient<IEndpoint, TEndpoint>();
            return services;
        }
    }
}
