using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Edvantix.Chassis.EF;

public static class MigrateDbContextExtensions
{
    private const string ActivitySourceName = "DbMigrations";
    private static readonly ActivitySource _activitySource = new(ActivitySourceName);

    private static async Task MigrateDbContextAsync<TContext>(
        this IServiceProvider services,
        Func<TContext, IServiceProvider, Task> seeder
    )
        where TContext : DbContext
    {
        using var scope = services.CreateScope();
        var scopeServices = scope.ServiceProvider;
        var logger = scopeServices.GetRequiredService<ILogger<TContext>>();
        var context = scopeServices.GetService<TContext>();

        using var activity = _activitySource.StartActivity(
            $"Migration operation {typeof(TContext).Name}"
        );

        try
        {
            logger.LogInformation(
                "Migrating database associated with context {DbContextName}",
                typeof(TContext).Name
            );

            var strategy = context?.Database.CreateExecutionStrategy();

            if (strategy is not null)
            {
                await strategy.ExecuteAsync(() => InvokeSeeder(seeder!, context, scopeServices));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while migrating the database used on context {DbContextName}",
                typeof(TContext).Name
            );

            activity?.AddException(ex);

            throw new InvalidOperationException(
                $"Database migration failed for {typeof(TContext).Name}. See inner exception for details.",
                ex
            );
        }
    }

    private static async Task InvokeSeeder<TContext>(
        Func<TContext, IServiceProvider, Task> seeder,
        TContext context,
        IServiceProvider services
    )
        where TContext : DbContext?
    {
        using var activity = _activitySource.StartActivity($"Migrating {typeof(TContext).Name}");

        try
        {
            await context?.Database.MigrateAsync()!;
            await seeder(context, services);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);

            throw;
        }
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует хостируемую миграцию базы данных для указанного <typeparamref name="TContext" /> без дополнительной логики заполнения.
        /// </summary>
        /// <typeparam name="TContext">Контекст базы данных EF Core для миграции.</typeparam>
        /// <returns>Та же коллекция сервисов для цепочки вызовов.</returns>
        public IServiceCollection AddMigration<TContext>()
            where TContext : DbContext
        {
            return services.AddMigration<TContext>((_, _) => Task.CompletedTask);
        }

        /// <summary>
        /// Регистрирует хостируемую миграцию базы данных для указанного <typeparamref name="TContext" />
        /// и выполняет переданный делегат заполнения после применения миграций.
        /// </summary>
        /// <typeparam name="TContext">Контекст базы данных EF Core для миграции.</typeparam>
        /// <param name="seeder">
        /// Делегат для заполнения данных после миграции с использованием разрешённого контекста и scoped-сервисов.
        /// </param>
        /// <returns>Та же коллекция сервисов для цепочки вызовов.</returns>
        /// <remarks>
        /// Метод также регистрирует трассировку OpenTelemetry для источника активностей миграции.
        /// </remarks>
        public IServiceCollection AddMigration<TContext>(
            Func<TContext, IServiceProvider, Task> seeder
        )
            where TContext : DbContext
        {
            services
                .AddOpenTelemetry()
                .WithTracing(tracing => tracing.AddSource(ActivitySourceName));

            return services.AddHostedService(sp => new MigrationHostedService<TContext>(
                sp,
                seeder
            ));
        }

        /// <summary>
        /// Регистрирует типизированный заполнитель базы данных и настраивает хостируемую миграцию для <typeparamref name="TContext" />.
        /// </summary>
        /// <typeparam name="TContext">Контекст базы данных EF Core для миграции.</typeparam>
        /// <typeparam name="TDbSeeder">
        /// Реализация заполнителя, используемая для заполнения мигрированной базы данных.
        /// </typeparam>
        /// <returns>Та же коллекция сервисов для цепочки вызовов.</returns>
        public IServiceCollection AddMigration<TContext, TDbSeeder>()
            where TContext : DbContext
            where TDbSeeder : class, IDbSeeder<TContext>
        {
            services.AddScoped<IDbSeeder<TContext>, TDbSeeder>();
            return services.AddMigration<TContext>(
                (context, sp) => sp.GetRequiredService<IDbSeeder<TContext>>().SeedAsync(context)
            );
        }
    }

    private sealed class MigrationHostedService<TContext>(
        IServiceProvider serviceProvider,
        Func<TContext, IServiceProvider, Task> seeder
    ) : BackgroundService
        where TContext : DbContext
    {
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return serviceProvider.MigrateDbContextAsync(seeder);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}

public interface IDbSeeder<in TContext>
    where TContext : DbContext
{
    Task SeedAsync(TContext context);
}
