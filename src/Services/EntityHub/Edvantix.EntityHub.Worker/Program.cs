using Edvantix.EntityHub.Worker;
using Edvantix.EntityHub.Worker.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.AddApplicationServices();

var host = builder.Build();
host.Run();
