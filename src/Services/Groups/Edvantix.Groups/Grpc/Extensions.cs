using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Groups.Grpc.Services;
using Edvantix.Organizational.Grpc.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Edvantix.Groups.Grpc;

internal static class Extensions
{
    public static void AddGrpcServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        builder.Services.AddGrpcHealthChecks();

        // gRPC-клиент Organizational для проверки разрешений.
        builder.Services.AddGrpcServiceReference<PermissionGrpcService.PermissionGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Organisational)
                .Build(),
            HealthStatus.Degraded
        );

        builder.Services.AddSingleton<IPermissionService, PermissionService>();
    }
}
