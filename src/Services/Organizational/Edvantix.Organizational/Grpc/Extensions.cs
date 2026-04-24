using Edvantix.Chassis.Security.TokenExchange;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Organizational.Grpc.Services.Profiles;
using Edvantix.Persona.Grpc.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Edvantix.Organizational.Grpc;

internal static class Extensions
{
    public static void AddGrpcServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        services
            .AddGrpcServiceReference<ProfileGrpcService.ProfileGrpcServiceClient>(
                HttpUtilities
                    .AsUrlBuilder()
                    .WithScheme(builder.GetScheme())
                    .WithHost(Constants.Aspire.Services.Persona)
                    .Build(),
                HealthStatus.Degraded
            )
            .AddAuthTokenExchange(Constants.Aspire.Services.Persona);

        services.AddSingleton<IProfileService, ProfileService>();
    }
}
