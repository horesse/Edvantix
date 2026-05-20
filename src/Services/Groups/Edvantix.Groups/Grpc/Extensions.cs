using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Curriculum.Grpc.Services;
using Edvantix.Groups.Grpc.Services;
using Edvantix.Groups.Grpc.Services.Courses;
using Edvantix.Groups.Grpc.Services.Profiles;
using Edvantix.Groups.Grpc.Services.Schedules;
using Edvantix.Organizational.Grpc.Services;
using Edvantix.Persona.Grpc.Services;
using Edvantix.Schedule.Grpc.Services;
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

        builder.Services.AddGrpcServiceReference<ProfileGrpcService.ProfileGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Persona)
                .Build(),
            HealthStatus.Degraded
        );

        builder.Services.AddGrpcServiceReference<CurriculumGrpcService.CurriculumGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Curriculum)
                .Build(),
            HealthStatus.Degraded
        );

        builder.Services.AddGrpcServiceReference<ScheduleGrpcService.ScheduleGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Schedule)
                .Build(),
            HealthStatus.Degraded
        );

        builder.Services.AddSingleton<IPermissionService, PermissionService>();
        builder.Services.AddSingleton<IProfileService, ProfileService>();
        builder.Services.AddSingleton<ICurriculumService, CurriculumService>();
        builder.Services.AddSingleton<IScheduleService, ScheduleService>();
    }
}
