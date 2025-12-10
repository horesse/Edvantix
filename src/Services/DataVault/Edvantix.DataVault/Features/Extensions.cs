using Edvantix.DataVault.Features.PlaygroundEntityFeature;

namespace Edvantix.DataVault.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        services.AddPlaygroundEntityFeature();
    }
}
