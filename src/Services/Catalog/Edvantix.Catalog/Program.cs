using Edvantix.Catalog.Extensions;
using Edvantix.Catalog.Grpc.Services;
using Edvantix.Chassis.Security.Keycloak;

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

app.MapGrpcService<CountryGrpcService>();
app.MapGrpcService<CurrencyGrpcService>();
app.MapGrpcService<LanguageGrpcService>();
app.MapGrpcService<RegionGrpcService>();
app.MapGrpcService<TimezoneGrpcService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
