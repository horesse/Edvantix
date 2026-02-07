using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.CQRS.Crud.Handlers.Commands;
using Edvantix.Chassis.CQRS.Crud.Handlers.Queries;
using Edvantix.Chassis.CQRS.Crud.Validators;
using Edvantix.Chassis.Specification;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.Results;
using Edvantix.SharedKernel.SeedWork;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS.Crud;

public static class MediatorCrudExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует все CRUD handlers для модели
        /// </summary>
        public IServiceCollection AddCrudHandlers<TModel, TIdentity, TEntity, TSpecification>()
            where TModel : class
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
        public IServiceCollection AddCrudHandlers<TModel, TIdentity, TEntity, TSpecification>(
            CrudActions actions
        )
            where TModel : class
            where TIdentity : struct
            where TEntity : Entity<TIdentity>, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            services.RegisterCrudHandlersCore<
                TModel,
                TModel,
                TModel,
                TIdentity,
                TEntity,
                TSpecification
            >(actions);

            services.AddScoped<
                IRequestHandler<ValidateCommand<TModel>, bool>,
                ValidateCommandHandler<TModel>
            >();

            services.AddValidators<TModel, TIdentity>();

            return services;
        }

        /// <summary>
        /// Регистрирует CRUD handlers с поддержкой ViewModels
        /// </summary>
        public IServiceCollection AddCrudViewModelHandlers<
            TModel,
            TCreateViewModel,
            TViewViewModel,
            TIdentity,
            TEntity,
            TSpecification
        >()
            where TModel : Model<TIdentity>
            where TCreateViewModel : class
            where TViewViewModel : class
            where TIdentity : struct
            where TEntity : Entity<TIdentity>, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            return services.AddCrudViewModelHandlers<
                TModel,
                TCreateViewModel,
                TViewViewModel,
                TIdentity,
                TEntity,
                TSpecification
            >(CrudActions.Create | CrudActions.GetById);
        }

        /// <summary>
        /// Регистрирует выборочные CRUD handlers с поддержкой ViewModels
        /// </summary>
        public IServiceCollection AddCrudViewModelHandlers<
            TModel,
            TCreateViewModel,
            TViewModel,
            TIdentity,
            TEntity,
            TSpecification
        >(CrudActions actions)
            where TModel : class
            where TCreateViewModel : class
            where TViewModel : class
            where TIdentity : struct
            where TEntity : Entity<TIdentity>, IAggregateRoot
            where TSpecification : class, ISpecification<TEntity>, new()
        {
            services.RegisterCrudHandlersCore<
                TModel,
                TCreateViewModel,
                TViewModel,
                TIdentity,
                TEntity,
                TSpecification
            >(actions);

            services.AddScoped<
                IRequestHandler<ValidateCommand<TCreateViewModel>, bool>,
                ValidateCommandHandler<TCreateViewModel>
            >();

            services.AddScoped<
                IRequestHandler<ValidateCommand<TModel>, bool>,
                ValidateCommandHandler<TModel>
            >();

            services.AddValidators<TModel, TIdentity>();

            return services;
        }

        public IServiceCollection AddCrudHandler<THandler, TRequest, TResponse>()
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

        /// <summary>
        /// Регистрирует общие CRUD query и command handlers для указанных типов.
        /// TCreateModel — тип для команды создания, TViewModel — тип для результатов GetByExpression.
        /// </summary>
        private IServiceCollection RegisterCrudHandlersCore<
            TModel,
            TCreateModel,
            TViewModel,
            TIdentity,
            TEntity,
            TSpecification
        >(CrudActions actions)
            where TModel : class
            where TCreateModel : class
            where TViewModel : class
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
                        GetByExpressionQuery<TEntity, TViewModel, TSpecification>,
                        PagedResult<TViewModel>
                    >,
                    GetByExpressionQueryHandler<TEntity, TViewModel, TSpecification, TIdentity>
                >();
            }

            // Command Handlers
            if (actions.HasFlag(CrudActions.Create))
            {
                services.AddScoped<
                    IRequestHandler<CreateCommand<TCreateModel, TIdentity>, TIdentity>,
                    CreateCommandHandler<TCreateModel, TIdentity, TEntity>
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

        private IServiceCollection AddValidators<TModel, TIdentity>()
            where TModel : class
            where TIdentity : struct
        {
            services.AddScoped<
                IValidator<CreateCommand<TModel, TIdentity>>,
                CreateCommandValidator<TModel, TIdentity>
            >();
            services.AddScoped<
                IValidator<UpdateCommand<TModel, TIdentity>>,
                UpdateCommandValidator<TModel, TIdentity>
            >();
            services.AddScoped<IValidator<ValidateCommand<TModel>>, CommandValidator<TModel>>();

            return services;
        }
    }
}
