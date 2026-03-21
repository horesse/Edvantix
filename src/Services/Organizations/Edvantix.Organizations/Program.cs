using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Organizations.Extensions;
using Edvantix.Organizations.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

builder.AddServiceDefaults();

builder.AddApplicationServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseExceptionHandler();

app.UseStatusCodePages();

app.UseDefaultCors();

app.UseKeycloakTokenIntrospection();

app.UseRateLimiter();

// Resolve tenant from X-School-Id header before authorization so that
// authorization handlers and DbContext query filters have the school context.
app.UseTenantContext();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(ApiVersions.V1)
    .ReportApiVersions()
    .Build();

app.MapEndpoints(apiVersionSet);

// Register the Organizations gRPC permission check service.
// CRITICAL: Without this mapping all gRPC calls return 404/Unimplemented.
app.MapGrpcService<PermissionsGrpcService>();

// Register the Organizations gRPC groups service.
// Used by Scheduling to resolve group memberships and validate group existence.
app.MapGrpcService<GroupsGrpcService>();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.UseAuthorization();

app.Run();
