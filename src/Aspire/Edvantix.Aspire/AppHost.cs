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

var dataVaultDb = postgres.AddDatabase(Components.Database.DataVault);

IResourceBuilder<IResource> keycloak = builder.ExecutionContext.IsRunMode
    ? builder.AddLocalKeycloak(Components.KeyCloak)
    : builder.AddHostedKeycloak(Components.KeyCloak);

var dataVaultApi = builder
    .AddProject<Edvantix_DataVault>(Services.DataVault)
    .WithReference(dataVaultDb)
    .WaitFor(dataVaultDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();

var gateway = builder.AddApiGatewayProxy().WithService(dataVaultApi).WithService(keycloak);

if (builder.ExecutionContext.IsRunMode)
{
    builder.AddScalar(keycloak).WithOpenAPI(dataVaultApi);
}

builder.Pipeline.AddGhcrPushStep();

await builder.Build().RunAsync();
