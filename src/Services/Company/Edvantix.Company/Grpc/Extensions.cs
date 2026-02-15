using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Security.TokenExchange;
using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Configuration;
using Edvantix.Company.Grpc.Services;
using Edvantix.ProfileService.Grpc.Services;
using Edvantix.ServiceDefaults.Kestrel;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Edvantix.Company.Grpc;

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

        services.AddGrpcServiceReference<ProfileGrpcService.ProfileGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Profile)
                .Build(),
            HealthStatus.Degraded
        );

        services.AddSingleton<IProfileService, Services.ProfileService>();
    }
}
