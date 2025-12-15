using Microsoft.Extensions.DependencyInjection;

namespace Edvantix.Chassis.Converter;

public static class Extensions
{
    public static void AddConverter(this IServiceCollection services, Type type)
    {
        services.Scan(scan =>
            scan.FromAssemblies(type.Assembly)
                .AddClasses(classes =>
                    classes.AssignableTo(typeof(IConverter<,>)).Where(t => !t.IsAbstract)
                )
                .AsImplementedInterfaces()
                .WithScopedLifetime()
        );
    }
}
