using Edvantix.Chassis.Security.Keycloak;
using Edvantix.Notification.Extensions;
using Edvantix.Notification.Grpc.Services;

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

// Аутентификация и авторизация для REST-эндпоинтов
app.UseAuthentication();

app.UseMiddleware<KeycloakTokenIntrospectionMiddleware>();

app.UseAuthorization();

// Версионированные REST-эндпоинты (доступны через API Gateway для фронтенда)
var apiVersionSet = app.NewApiVersionSet().HasApiVersion(new(1, 0)).ReportApiVersions().Build();

app.MapEndpoints(apiVersionSet);

// gRPC-эндпоинты (для межсервисного создания in-app уведомлений)
app.MapGrpcService<InAppNotificationGrpcService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.UseDefaultOpenApi();

app.Run();
