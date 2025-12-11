using Edvantix.Aspire.Extensions.Infrastructure;
using Edvantix.Aspire.Extensions.Network;
using Edvantix.Aspire.Extensions.Security;
using Edvantix.Constants.Aspire;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment(Components.Azure.ContainerApp).ProvisionAsService();

var postgres = builder
    .AddAzurePostgresFlexibleServer(Components.Postgres)
    .WithPasswordAuthentication()
    .WithIconName("HomeDatabase")
    .RunAsLocalContainer()
    .ProvisionAsService();

// var redis = builder
//     .AddAzureRedis(Components.Redis)
//     .WithAccessKeyAuthentication()
//     .WithIconName("Memory")
//     .RunAsLocalContainer()
//     .ProvisionAsService();
//
// var qdrant = builder
//     .AddQdrant(Components.VectorDb)
//     .WithIconName("DatabaseSearch")
//     .WithDataVolume()
//     .WithImagePullPolicy(ImagePullPolicy.Always)
//     .WithLifetime(ContainerLifetime.Persistent);
//
// var queue = builder
//     .AddRabbitMQ(Components.Queue)
//     .WithIconName("Pipeline")
//     .WithManagementPlugin()
//     .WithDataVolume()
//     .WithImagePullPolicy(ImagePullPolicy.Always)
//     .WithLifetime(ContainerLifetime.Persistent)
//     .WithEndpoint(Network.Tcp, e => e.Port = 5672);
//
// var storage = builder
//     .AddAzureStorage(Components.Azure.Storage.Resource)
//     .WithIconName("DatabasePlugConnected")
//     .RunAsLocalContainer()
//     .ProvisionAsService();

var dataVaultDb = postgres.AddDatabase(Components.Database.DataVault);
var entityHubDb = postgres.AddDatabase(Components.Database.EntityHub);

IResourceBuilder<IResource> keycloak = builder.ExecutionContext.IsRunMode
    ? builder.AddLocalKeycloak(Components.KeyCloak)
    : builder.AddHostedKeycloak(Components.KeyCloak);

var dataVaultApi = builder
    .AddProject<Edvantix_DataVault>(Services.DataVault)
    .WithReference(dataVaultDb)
    .WaitFor(dataVaultDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();

var entityHubApi = builder
    .AddProject<Edvantix_EntityHub>(Services.EntityHub)
    .WithReference(entityHubDb)
    .WaitFor(entityHubDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();

builder
    .AddProject<Edvantix_EntityHub_Worker>(Services.EntityHubWorker)
    .WaitFor(entityHubApi)
    .WithReference(entityHubDb)
    .WaitFor(entityHubDb);

var gateway = builder
    .AddApiGatewayProxy()
    .WithService(dataVaultApi)
    .WithService(entityHubApi)
    .WithService(keycloak);

if (builder.ExecutionContext.IsRunMode)
{
    builder.AddScalar(keycloak).WithOpenAPI(dataVaultApi).WithOpenAPI(entityHubApi);
}

builder.Pipeline.AddGhcrPushStep();

await builder.Build().RunAsync();
