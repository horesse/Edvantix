using CritterWatch.Services.Hosting;
using Edvantix.Chassis.EventBus.Wolverine;
using Edvantix.Constants.Aspire;
using Wolverine.Kafka;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString(Components.Database.CritterWatch)
    ?? throw new Exception("CritterWatch connection string is not set");

var kafkaConnectionString = builder.Configuration.GetConnectionString(Components.Broker);

if (string.IsNullOrWhiteSpace(kafkaConnectionString))
    return;

var applicationName = "CritterWatch";

builder.AddCritterWatch(
    connectionString,
    configureWolverine: opts =>
    {
        opts.UseKafka(kafkaConnectionString).AutoProvision();

        opts.UseKafkaWithCloudEvents(kafkaConnectionString, applicationName);

        opts.ListenToKafkaTopic("critterwatch").ListenOnlyAtLeader();
    },
    enableClusterPartitioning: false
);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCritterWatch();
app.MapHealthChecks("/health");

app.Run();
