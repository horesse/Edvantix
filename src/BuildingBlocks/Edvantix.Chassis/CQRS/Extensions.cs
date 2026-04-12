using Edvantix.Chassis.CQRS.Command;
using Edvantix.Chassis.CQRS.Pipelines;
using Edvantix.Chassis.CQRS.Query;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.CQRS;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует коллектор метрик обработчика команд в контейнере зависимостей.
        /// </summary>
        /// <returns>
        /// Текущая коллекция сервисов.
        /// </returns>
        public IServiceCollection AddCommandHandlerMetrics()
        {
            services.AddSingleton<CommandHandlerMetrics>();
            return services;
        }

        /// <summary>
        /// Регистрирует коллектор метрик обработчика запросов в контейнере зависимостей.
        /// </summary>
        /// <returns>
        /// Текущая коллекция сервисов.
        /// </returns>
        public IServiceCollection AddQueryHandlerMetrics()
        {
            services.AddSingleton<QueryHandlerMetrics>();
            return services;
        }

        /// <summary>
        /// Регистрирует поведение конвейера для трассировки активностей.
        /// </summary>
        /// <returns>
        /// Текущая коллекция сервисов.
        /// </returns>
        public IServiceCollection ApplyActivityBehavior()
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ActivityBehavior<,>));
            return services;
        }

        /// <summary>
        /// Регистрирует поведение конвейера для логирования.
        /// </summary>
        /// <returns>
        /// Текущая коллекция сервисов.
        /// </returns>
        public IServiceCollection ApplyLoggingBehavior()
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            return services;
        }

        /// <summary>
        /// Регистрирует поведение конвейера для валидации.
        /// </summary>
        /// <returns>
        /// Текущая коллекция сервисов.
        /// </returns>
        public IServiceCollection ApplyValidationBehavior()
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            return services;
        }

        /// <summary>
        /// Регистрирует поведение конвейера для управления транзакциями для указанного контекста базы данных.
        /// </summary>
        /// <typeparam name="TDbContext">
        /// Тип контекста базы данных, используемый для разрешения транзакционного контекста.
        /// </typeparam>
        /// <returns>
        /// Текущая коллекция сервисов.
        /// </returns>
        public IServiceCollection ApplyTransactionBehavior<TDbContext>()
            where TDbContext : DbContext
        {
            services.AddScoped<DbContext>(sp => sp.GetRequiredService<TDbContext>());
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
            return services;
        }
    }
}
