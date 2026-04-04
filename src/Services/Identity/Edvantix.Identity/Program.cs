using Edvantix.Identity.Extensions;
using Edvantix.Identity.Grpc.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o => o.AddServerHeader = false);

builder.AddServiceDefaults();

builder.AddApplicationServices();

var app = builder.Build();

app.UseExceptionHandler();

app.MapGrpcService<IdentityService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.Run();
