using Edvantix.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.EF;

public static class DbContextExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует PostgreSQL <see cref="DbContext" />, настроенный для Azure, с именованием snake_case,
        /// запросами без отслеживания и опциональными EF Core перехватчиками.
        /// </summary>
        /// <typeparam name="TDbContext">Тип <see cref="DbContext" /> для регистрации.</typeparam>
        /// <param name="name">Имя строки подключения из конфигурации.</param>
        /// <param name="action">Необязательный обратный вызов для дополнительной настройки построителя.</param>
        /// <param name="excludeDefaultInterceptors">
        /// Установите <see langword="true" /> для пропуска регистрации перехватчиков по умолчанию.
        /// </param>
        public void AddAzurePostgresDbContext<TDbContext>(
            string name,
            Action<IHostApplicationBuilder>? action = null,
            bool excludeDefaultInterceptors = false
        )
            where TDbContext : DbContext
        {
            var services = builder.Services;

            // Регистрирует сквозные перехватчики по умолчанию для диагностики запросов и диспетчеризации доменных событий.
            if (!excludeDefaultInterceptors)
            {
                services.AddScoped<IInterceptor, QueryPerformanceInterceptor>();
                services.AddScoped<IInterceptor, EventDispatchInterceptor>();
                services.AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();
            }

            services.AddDbContext<TDbContext>(
                (sp, options) =>
                {
                    options
                        .UseNpgsql(builder.Configuration.GetConnectionString(name))
                        .UseSnakeCaseNamingConvention()
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                        // Подавляет известное предупреждение EF Core о незафиксированных изменениях модели.
                        // Issue: https://github.com/dotnet/efcore/issues/35285
                        .ConfigureWarnings(warnings =>
                            warnings.Ignore(RelationalEventId.PendingModelChangesWarning)
                        );

                    // Разрешает и применяет все зарегистрированные перехватчики EF Core.
                    var interceptors = sp.GetServices<IInterceptor>().ToArray();

                    if (interceptors.Length != 0)
                    {
                        options.AddInterceptors(interceptors);
                    }
                }
            );

            // Применяет специфическое для проекта обогащение Azure Npgsql.
            builder.EnrichAzureNpgsqlDbContext<TDbContext>();

            action?.Invoke(builder);
        }
    }
}
