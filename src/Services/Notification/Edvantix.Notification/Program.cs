using Edvantix.Chassis.Endpoints;
using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Constants.Core;
using Edvantix.Notification.Extensions;
using Edvantix.ServiceDefaults.ApiSpecification.OpenApi;
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

app.UseAuthorization();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(ApiVersions.V1)
    .ReportApiVersions()
    .Build();

app.MapEndpoints(apiVersionSet);

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
