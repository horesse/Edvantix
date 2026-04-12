using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Mapper;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует все неабстрактные реализации маппера из указанной сборки в контейнере зависимостей.
        /// </summary>
        /// <param name="type">
        /// Тип для определения целевой сборки при сканировании реализаций <see cref="IMapper{TSource,TDestination}" />.
        /// </param>
        public void AddMapper(Type type)
        {
            services.Scan(scan =>
                scan.FromAssemblies(type.Assembly)
                    .AddClasses(
                        classes =>
                            classes.AssignableTo(typeof(IMapper<,>)).Where(t => !t.IsAbstract),
                        false
                    )
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );
        }
    }
}
