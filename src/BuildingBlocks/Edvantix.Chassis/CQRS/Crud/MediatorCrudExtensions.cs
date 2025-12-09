using Edvantix.Chassis.CQRS.Crud.Abstractions;
using Edvantix.Chassis.CQRS.Crud.Handlers.Commands;
using Edvantix.Chassis.CQRS.Crud.Handlers.Queries;
using Edvantix.SharedKernel.SeedWork;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS.Crud;

public static class MediatorCrudExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует все CRUD handlers для конкретной модели
        /// </summary>
        public IServiceCollection AddCrudHandlers<TModel, TIdentity, TEntity>()
            where TModel : Model<TIdentity>
            where TIdentity : struct
            where TEntity : Entity<TIdentity>, IAggregateRoot
        {
            // Query Handlers
            services.AddScoped<IQueryHandler<GetAllQuery<TModel, TIdentity>, IEnumerable<TModel>>,
                GetAllQueryHandler<TModel, TIdentity, TEntity>>();
        
            services.AddScoped<IQueryHandler<GetAllByIdsQuery<TModel, TIdentity>, IEnumerable<TModel>>,
                GetAllByIdsQueryHandler<TModel, TIdentity, TEntity>>();
        
            services.AddScoped<IQueryHandler<GetByIdQuery<TModel, TIdentity>, TModel>,
                GetByIdQueryHandler<TModel, TIdentity, TEntity>>();

            services.AddScoped<IQueryHandler<GetCountQuery, long>,
                GetCountQueryHandler<TModel, TIdentity, TEntity>>();

            services.AddScoped<IQueryHandler<IsExistQuery<TIdentity>, bool>,
                IsExistQueryHandler<TModel, TIdentity, TEntity>>();

            // Command Handlers
            services.AddScoped<ICommandHandler<CreateCommand<TModel, TIdentity>, TIdentity>,
                CreateCommandHandler<TModel, TIdentity, TEntity>>();

            services.AddScoped<ICommandHandler<CreateRangeCommand<TModel, TIdentity>, IEnumerable<TIdentity>>,
                CreateRangeCommandHandler<TModel, TIdentity, TEntity>>();
        
            services.AddScoped<ICommandHandler<UpdateCommand<TModel, TIdentity>, TIdentity>,
                UpdateCommandHandler<TModel, TIdentity, TEntity>>();

            services.AddScoped<ICommandHandler<UpdateRangeCommand<TModel, TIdentity>, IEnumerable<TIdentity>>,
                UpdateRangeCommandHandler<TModel, TIdentity, TEntity>>();
        
            services.AddScoped<ICommandHandler<DeleteCommand<TModel, TIdentity>, TIdentity>,
                DeleteCommandHandler<TModel, TIdentity, TEntity>>();

            services.AddScoped<ICommandHandler<DeleteRangeCommand<TModel, TIdentity>, IEnumerable<TIdentity>>,
                DeleteRangeCommandHandler<TModel, TIdentity, TEntity>>();

            return services;
        }

        /// <summary>
        /// Регистрирует кастомный handler, переопределяя стандартный
        /// </summary>
        public IServiceCollection AddCrudHandler<THandler, TRequest, TResponse>()
            where THandler : class, IRequestHandler<TRequest, TResponse>
            where TRequest : IRequest<TResponse>
        {
            // Удаляем существующую регистрацию если есть
            var descriptor = services.FirstOrDefault(d => 
                d.ServiceType == typeof(IRequestHandler<TRequest, TResponse>));
        
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddScoped<IRequestHandler<TRequest, TResponse>, THandler>();
            return services;
        }
    }
}
