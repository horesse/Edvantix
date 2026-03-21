using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Organizations.Grpc.Generated;
using Edvantix.Persona.Grpc.Services;
using Edvantix.Scheduling.Grpc.Services;

namespace Edvantix.Scheduling.Grpc;

/// <summary>
/// Registers gRPC clients consumed by the Scheduling service.
/// Wires the Persona profile client and the Organizations groups client.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Registers gRPC clients and their service abstractions.
    /// </summary>
    public static void AddGrpcServices(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Register Persona gRPC client with health degradation on unavailability.
        // The URL is resolved from Aspire service discovery using the "persona" service name.
        services.AddGrpcServiceReference<ProfileGrpcService.ProfileGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Persona)
                .Build(),
            HealthStatus.Degraded
        );

        services.AddSingleton<IPersonaProfileService, PersonaProfileService>();

        // Register Organizations gRPC client for group validation and student membership resolution.
        // Replaced the HTTP-based OrganizationsGroupService from Plan 03-04 with a proper gRPC client.
        services.AddGrpcServiceReference<GroupsGrpcService.GroupsGrpcServiceClient>(
            HttpUtilities
                .AsUrlBuilder()
                .WithScheme(builder.GetScheme())
                .WithHost(Constants.Aspire.Services.Organizations)
                .Build(),
            HealthStatus.Degraded
        );

        // Scoped (not Singleton) because the gRPC client may carry per-request metadata in future.
        services.AddScoped<IOrganizationsGroupService, OrganizationsGroupService>();
    }
}
