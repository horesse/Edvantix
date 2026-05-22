namespace Edvantix.Organizational.Features.Settings.Directories;

/// <summary>
/// Регистрирует <see cref="IDirectoryStatsProvider"/> в DI.
/// </summary>
internal static class DirectoryProviderExtensions
{
    /// <summary>
    /// Регистрирует <see cref="StubDirectoryStatsProvider"/> для каждого справочника из
    /// <see cref="DirectoryCatalog.All"/>. Реальные провайдеры (Tasks 4–11) должны
    /// регистрироваться ПОСЛЕ этого вызова — тогда при построении словаря в handler'е
    /// они переопределят заглушки (last-registration-wins).
    /// </summary>
    internal static IServiceCollection AddDirectoryProviders(this IServiceCollection services)
    {
        foreach (var descriptor in DirectoryCatalog.All)
        {
            services.AddSingleton<IDirectoryStatsProvider>(
                new StubDirectoryStatsProvider(descriptor)
            );
        }

        return services;
    }
}
