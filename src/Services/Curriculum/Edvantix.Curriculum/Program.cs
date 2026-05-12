using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Curriculum.Extensions;
using Edvantix.Curriculum.Grpc.Services;
using Edvantix.Curriculum.Grpc.Services.Curriculum;
using Edvantix.ServiceDefaults.Cors;

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

app.UseTenantContext();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(ApiVersions.V1)
    .ReportApiVersions()
    .Build();

app.MapEndpoints(apiVersionSet);

app.MapGrpcService<CurriculumCatalogService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.UseAuthorization();

app.Run();
