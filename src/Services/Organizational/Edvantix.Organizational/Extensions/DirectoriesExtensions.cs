using Edvantix.Organizational.Features.Settings.Directories;

namespace Edvantix.Organizational.Extensions;

internal static class DirectoriesExtensions
{
    /// <summary>
    /// Регистрирует сервисы справочников: провайдеры статистики (<see cref="IDirectoryStatsProvider"/>)
    /// и чекеры уникальности имён (<see cref="IUniqueNameChecker"/>) через Scrutor-сканирование.
    /// </summary>
    public static IServiceCollection AddDirectoryServices(
        this IServiceCollection services,
        Type markerType
    )
    {
        // Провайдеры регистрируются как IDirectoryStatsProvider,
        // чтобы их можно было инжектировать через IEnumerable<IDirectoryStatsProvider>
        services.Scan(scan =>
            scan.FromAssembliesOf(markerType)
                .AddClasses(
                    classes => classes.AssignableTo<IDirectoryStatsProvider>(),
                    publicOnly: false
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );

        // Чекеры уникальности регистрируются как конкретный тип (AsSelf),
        // потому что каждый валидатор принимает свою конкретную реализацию
        services.Scan(scan =>
            scan.FromAssembliesOf(markerType)
                .AddClasses(
                    classes => classes.AssignableTo<IUniqueNameChecker>(),
                    publicOnly: false
                )
                .AsSelf()
                .WithScopedLifetime()
        );

        return services;
    }
}
