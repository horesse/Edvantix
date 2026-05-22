using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Curriculum.Grpc.Services;
using Edvantix.Groups.Grpc.Services;
using Edvantix.Organizational.Grpc.Services.Courses;
using Edvantix.Organizational.Grpc.Services.Groups;
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

        services.AddGrpcServiceReference<ProfileGrpcService.ProfileGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Persona)
                .Build(),
            HealthStatus.Degraded
        );

        services.AddGrpcServiceReference<CurriculumGrpcService.CurriculumGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Curriculum)
                .Build(),
            HealthStatus.Degraded
        );

        services.AddGrpcServiceReference<GroupsGrpcService.GroupsGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Groups)
                .Build(),
            HealthStatus.Degraded
        );

        services.AddSingleton<IProfileService, ProfileService>();
        services.AddSingleton<ICurriculumService, CurriculumService>();
        services.AddSingleton<IGroupsService, GroupsService>();
    }
}
