using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Organizations.Extensions;

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

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.UseAuthorization();

app.Run();
