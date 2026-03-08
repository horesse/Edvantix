using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Persona.Extensions;
using Edvantix.Persona.Grpc.Services;

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

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(ApiVersions.V1)
    .ReportApiVersions()
    .Build();

app.MapEndpoints(apiVersionSet);

app.MapGrpcService<ProfileService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.UseAuthorization();

app.Run();
