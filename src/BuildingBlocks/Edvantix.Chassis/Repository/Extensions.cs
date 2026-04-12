using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Repository;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует все реализации репозиториев из сборки, содержащей указанный тип.
        /// </summary>
        /// <param name="type">Тип, используемый для определения целевой сборки при поиске репозиториев.</param>
        public void AddRepositories(Type type)
        {
            services.Scan(scan =>
                scan.FromAssembliesOf(type)
                    .AddClasses(classes => classes.AssignableTo(typeof(IRepository<>)), false)
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
            );
        }
    }
}
