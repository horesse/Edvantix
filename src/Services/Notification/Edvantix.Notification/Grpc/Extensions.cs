namespace Edvantix.Notification.Grpc;

/// <summary>
/// Регистрация gRPC-сервисов уведомлений.
/// </summary>
internal static class Extensions
{
    public static void AddNotificationGrpcServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = builder.Environment.IsDevelopment();
            options.Interceptors.Add<GrpcExceptionInterceptor>();
        });

        services.AddGrpcHealthChecks();
    }
}
