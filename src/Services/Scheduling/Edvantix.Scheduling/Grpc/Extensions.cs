using Edvantix.Chassis.Utilities;
using Edvantix.Chassis.Utilities.Configurations;
using Edvantix.Persona.Grpc.Services;
using Edvantix.Scheduling.Grpc.Services;

namespace Edvantix.Scheduling.Grpc;

/// <summary>
/// Registers gRPC clients consumed by the Scheduling service.
/// Currently wires the Persona profile client used to validate profileId before teacher/student assignment.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Registers the Persona gRPC client and the <see cref="IPersonaProfileService"/> singleton.
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
    }
}
