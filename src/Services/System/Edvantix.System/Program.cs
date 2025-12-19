using Edvantix.Chassis.Endpoints;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.ServiceDefaults;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi;
using Edvantix.ServiceDefaults.Kestrel;
using Edvantix.System.Extensions;
using Edvantix.System.Grpc.Services;

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

app.UseMiddleware<KeycloakTokenIntrospectionMiddleware>();

app.UseRateLimiter();

var apiVersionSet = app.NewApiVersionSet().HasApiVersion(new(1, 0)).ReportApiVersions().Build();

app.MapEndpoints(apiVersionSet);

app.MapGrpcService<LimitService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
