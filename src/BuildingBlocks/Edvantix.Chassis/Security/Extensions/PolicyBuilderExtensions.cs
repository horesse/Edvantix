using Edvantix.Chassis.Security.Authorization;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Organizations.Grpc.Generated;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Security.Extensions;

public static class PolicyBuilderExtensions
{
    /// <summary>
    /// Добавляет требование наличия claim <c>profile_id</c> в токене.
    /// Используйте <see cref="AddProfileRequiredServices"/> для регистрации обработчиков.
    /// </summary>
    public static AuthorizationPolicyBuilder RequireProfileRegistered(
        this AuthorizationPolicyBuilder builder
    ) => builder.AddRequirements(new ProfileRegisteredRequirement());

    /// <summary>
    /// Adds a <see cref="PermissionRequirement"/> to the authorization policy.
    /// The requirement is resolved at runtime by calling the Organizations gRPC service.
    /// Pair with <see cref="AddPermissionAuthorization"/> during service registration.
    /// </summary>
    /// <param name="builder">The authorization policy builder.</param>
    /// <param name="permission">
    /// The permission string to check (e.g., <c>"scheduling.create-slot"</c>).
    /// </param>
    /// <example>
    /// <code>
    /// builder.Services.AddAuthorization(options =>
    ///     options.AddPolicy("CanCreateSlot", policy => policy.RequirePermission("scheduling.create-slot")));
    /// </code>
    /// </example>
    public static AuthorizationPolicyBuilder RequirePermission(
        this AuthorizationPolicyBuilder builder,
        string permission
    ) => builder.AddRequirements(new PermissionRequirement(permission));

    /// <summary>
    /// Registers the Organizations gRPC client and <see cref="PermissionRequirementHandler"/>
    /// for permission-based authorization. Call this in downstream services (Scheduling, Payments)
    /// before calling <c>AddAuthorization</c> policies that use <see cref="RequirePermission"/>.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="organizationsGrpcBaseUrl">
    /// The base URL (or Aspire service name) for the Organizations gRPC service.
    /// </param>
    /// <example>
    /// <code>
    /// builder.AddPermissionAuthorization("https+http://organizations");
    /// </code>
    /// </example>
    public static IServiceCollection AddPermissionAuthorization(
        this IHostApplicationBuilder builder,
        string organizationsGrpcBaseUrl
    )
    {
        // Register the gRPC client pointing at the Organizations service.
        builder.Services.AddGrpcClient<PermissionsGrpcService.PermissionsGrpcServiceClient>(o =>
            o.Address = new Uri(organizationsGrpcBaseUrl)
        );

        // IHttpContextAccessor is required by PermissionRequirementHandler to resolve
        // the scoped ITenantContext during authorization evaluation.
        builder.Services.AddHttpContextAccessor();

        // Register as singleton — gRPC client is thread-safe and handler has no per-request state.
        builder.Services.AddSingleton<IAuthorizationHandler, PermissionRequirementHandler>();

        return builder.Services;
    }

    /// <summary>
    /// Регистрирует обработчик требования <see cref="ProfileRegisteredRequirement"/>
    /// и кастомный <see cref="IAuthorizationMiddlewareResultHandler"/>,
    /// который возвращает <c>PROFILE_NOT_REGISTERED</c> при отсутствии профиля.
    /// </summary>
    public static IServiceCollection AddProfileRequiredServices(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, ProfileRegisteredRequirementHandler>();
        services.AddSingleton<
            IAuthorizationMiddlewareResultHandler,
            ProfileAuthorizationResultHandler
        >();
        return services;
    }

    public static AuthorizationPolicyBuilder RequireScope(
        this AuthorizationPolicyBuilder authorizationPolicyBuilder,
        params string[] allowedValues
    )
    {
        var scopeClaim = authorizationPolicyBuilder.RequireAssertion(context =>
        {
            var scopeClaim = context.User.FindFirst(KeycloakClaimTypes.Scope);

            if (scopeClaim is null)
            {
                return false;
            }

            var scopes = scopeClaim.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return scopes.Any(s => allowedValues.Contains(s, StringComparer.OrdinalIgnoreCase));
        });

        return scopeClaim;
    }
}
