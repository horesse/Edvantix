using Edvantix.EntityHub.Infrastructure;

namespace Edvantix.EntityHub.Worker.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        builder.AddPersistenceServices(false);
    }
}
