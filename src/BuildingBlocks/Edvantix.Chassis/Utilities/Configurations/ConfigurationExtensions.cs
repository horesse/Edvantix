using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Edvantix.Chassis.Utilities.Configurations;

public static class ConfigurationExtensions
{
    extension(IConfiguration configuration)
    {
        /// <summary>
        /// Считывает именованную строку подключения из конфигурации и выбрасывает исключение, если значение отсутствует или пустое.
        /// </summary>
        /// <param name="name">Ключ строки подключения в секции <c>ConnectionStrings</c>.</param>
        /// <returns>Значение строки подключения из конфигурации.</returns>
        /// <exception cref="InvalidOperationException">
        /// Выбрасывается, когда строка подключения не найдена или пуста.
        /// </exception>
        public string GetRequiredConnectionString(string name)
        {
            var connectionString = configuration.GetConnectionString(name);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    $"Configuration missing value for: {(configuration is IConfigurationSection s ? s.Path + ":" + name : name)}"
                );
            }

            return connectionString;
        }
    }

    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует, привязывает и валидирует тип настроек из секции конфигурации.
        /// </summary>
        /// <typeparam name="TSetting">Тип настроек для привязки.</typeparam>
        /// <param name="section">Путь к секции конфигурации.</param>
        /// <param name="name">Необязательный именованный экземпляр options.</param>
        /// <param name="configure">Необязательный обратный вызов для применения дополнительной in-memory конфигурации.</param>
        /// <remarks>
        /// Этот метод включает валидацию при запуске и валидацию data annotations, а затем
        /// публикует полученный экземпляр настроек как singleton для прямого внедрения зависимостей.
        /// </remarks>
        public void Configure<TSetting>(
            string section,
            string? name = null,
            Action<TSetting>? configure = null
        )
            where TSetting : class
        {
            var services = builder.Services;

            services
                .AddOptionsWithValidateOnStart<TSetting>(name)
                .Configure(options => configure?.Invoke(options))
                .BindConfiguration(section)
                .ValidateDataAnnotations();

            // Публикует привязанное значение options напрямую для потребителей, зависящих от TSetting.
            services.TryAddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<TSetting>>();
                var setting = options.Value;
                return setting;
            });
        }

        /// <summary>
        /// Разбирает и регистрирует настройки приложения как singleton-экземпляр.
        /// </summary>
        /// <typeparam name="T">Конкретный тип настроек приложения.</typeparam>
        /// <returns>Обновлённая коллекция сервисов.</returns>
        /// <remarks>
        /// Этот вспомогательный метод использует <see cref="AppSettings.Parse{T}(IConfiguration)" />,
        /// чтобы один раз материализовать конфигурацию при запуске и зарегистрировать её для DI-потребителей.
        /// </remarks>
        public IServiceCollection AddAppSettings<T>()
            where T : AppSettings, new()
        {
            var services = builder.Services;

            services.AddSingleton<T>(_ =>
            {
                var settings = AppSettings.Parse<T>(builder.Configuration);
                return settings;
            });

            return services;
        }
    }
}
