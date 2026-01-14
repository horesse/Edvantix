using Edvantix.Aspire.Extensions.Infrastructure;
using Edvantix.Aspire.Extensions.Network;
using Edvantix.Aspire.Extensions.Security;
using Edvantix.Constants.Aspire;
using Edvantix.Constants.Core;
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
var organizationDb = postgres.AddDatabase(Components.Database.Organization);
var systemDb = postgres.AddDatabase(Components.Database.System);
var personDb = postgres.AddDatabase(Components.Database.Person);

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

var organizationApi = builder
    .AddProject<Edvantix_Company>(Services.Company)
    .WithReference(organizationDb)
    .WaitFor(organizationDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();

var systemApi = builder
    .AddProject<Edvantix_System>(Services.System)
    .WithReference(systemDb)
    .WaitFor(systemDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();

var personApi = builder
    .AddProject<Edvantix_Person>(Services.Person)
    .WithReference(personDb)
    .WaitFor(personDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();

var gateway = builder
    .AddApiGatewayProxy()
    .WithService(dataVaultApi)
    .WithService(entityHubApi)
    .WithService(organizationApi, true)
    .WithService(systemApi, true)
    .WithService(personApi, true)
    .WithService(keycloak);

var turbo = builder
    .AddTurborepoApp(
        Components.TurboRepo,
        Path.GetFullPath("../../Clients", builder.AppHostDirectory)
    )
    .WithPnpm(true)
    .WithPackageManagerLaunch();

var front = turbo
    .AddApp(Clients.OrganizationFront, Clients.OrganizationTurboApp)
    .WithOtlpExporter()
    .WithHttpEndpoint(env: "PORT")
    .WithMappedEndpointPort()
    .WithHttpHealthCheck()
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Http.Schemes.Https))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Http.Schemes.Http))
    .WithEnvironment("NEXT_PUBLIC_COPILOT_ENABLED", "true")
    .WithKeycloak(keycloak);

front.WithEnvironment("NEXT_PUBLIC_APP_URL", front.GetEndpoint(Http.Schemes.Http));

if (builder.ExecutionContext.IsRunMode)
{
    builder
        .AddScalar(keycloak)
        .WithOpenAPI(dataVaultApi)
        .WithOpenAPI(entityHubApi)
        .WithOpenAPI(organizationApi)
        .WithOpenAPI(systemApi)
        .WithOpenAPI(personApi);
}

builder.Pipeline.AddGhcrPushStep();

await builder.Build().RunAsync();
