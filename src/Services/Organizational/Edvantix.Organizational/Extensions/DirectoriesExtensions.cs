using Edvantix.Organizational.Features.Directories.LessonTypes;
using Edvantix.Organizational.Features.Settings.Directories;
using Edvantix.Organizational.Features.Settings.Directories.Catalog;

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
        // StubDirectoryStatsProvider исключён: создаётся вручную через new в хендлере,
        // т.к. требует DirectoryDescriptor в конструкторе — не является DI-сервисом.
        // Остальные провайдеры регистрируются как IDirectoryStatsProvider для IEnumerable<> инжекции.
        services.Scan(scan =>
            scan.FromAssembliesOf(markerType)
                .AddClasses(
                    classes =>
                        classes
                            .AssignableTo<IDirectoryStatsProvider>()
                            .Where(t => t != typeof(StubDirectoryStatsProvider)),
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

        // ILessonTypeUniqueChecker проверяет уникальность и по имени, и по коду —
        // не вписывается в IUniqueNameChecker, поэтому регистрируется явно.
        services.AddScoped<ILessonTypeUniqueChecker, LessonTypeUniqueChecker>();

        return services;
    }
}
