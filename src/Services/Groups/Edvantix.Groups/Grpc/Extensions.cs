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
    }
}
