using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Exceptions;
using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Configuration;
using Edvantix.ProfileService.Grpc.Services;
using Edvantix.ServiceDefaults.Kestrel;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Edvantix.Blog.Grpc;

/// <summary>
/// Регистрация gRPC-клиентов для микросервиса Blog.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Регистрирует gRPC-клиент Profile и связанные сервисы.
    /// </summary>
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
                .WithHost(Constants.Aspire.Services.Persona)
                .Build(),
            HealthStatus.Degraded
        );

        services.AddSingleton<IProfileService, Services.ProfileService>();
    }
}
