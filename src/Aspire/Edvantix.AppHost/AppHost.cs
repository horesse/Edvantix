var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureContainerAppEnvironment(Components.Azure.ContainerApp).ProvisionAsService();

var registry = builder.AddContainerRegistry();

var postgres = builder
    .AddAzurePostgresFlexibleServer(Components.Postgres)
    .WithPasswordAuthentication()
    .WithIconName("HomeDatabase")
    .RunAsLocalContainer()
    .ProvisionAsService();

var redis = builder
    .AddAzureManagedRedis(Components.Redis)
    .WithAccessKeyAuthentication()
    .WithIconName("Memory")
    .RunAsLocalContainer()
    .ProvisionAsService();

var qdrant = builder
    .AddQdrant(Components.VectorDb)
    .WithIconName("DatabaseSearch")
    .WithDataVolume()
    .WithImagePullPolicy(ImagePullPolicy.Always)
    .WithLifetime(ContainerLifetime.Persistent);

var queue = builder
    .AddRabbitMQ(Components.Queue)
    .WithIconName("Pipeline")
    .WithManagementPlugin()
    .WithDataVolume()
    .WithImagePullPolicy(ImagePullPolicy.Always)
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(Network.Tcp, e => e.Port = 5672);

var storage = builder
    .AddAzureStorage(Components.Azure.Storage.Resource)
    .WithIconName("DatabasePlugConnected")
    .RunAsLocalContainer()
    .ProvisionAsService();

var profileContainer = storage.AddBlobContainer(
    Components.Azure.Storage.BlobContainer(Services.Profile)
);

var entityHubDb = postgres.AddDatabase(Components.Database.EntityHub);
var organizationDb = postgres.AddDatabase(Components.Database.Organization);
var systemDb = postgres.AddDatabase(Components.Database.System);
var profileDb = postgres.AddDatabase(Components.Database.Profile);
var subscriptionDb = postgres.AddDatabase(Components.Database.Subscription);
var blogDb = postgres.AddDatabase(Components.Database.Blog);

IResourceBuilder<IResource> keycloak = builder.ExecutionContext.IsRunMode
    ? builder.AddLocalKeycloak(Components.KeyCloak)
    : builder.AddHostedKeycloak(Components.KeyCloak);

var profileApi = builder
    .AddProject<Edvantix_ProfileService>(Services.Profile)
    .WithReference(profileDb)
    .WaitFor(profileDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(profileContainer)
    .WaitFor(profileContainer)
    .WithFriendlyUrls();

var organizationApi = builder
    .AddProject<Edvantix_Company>(Services.Company)
    .WithReference(organizationDb)
    .WaitFor(organizationDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(profileApi)
    .WaitFor(profileApi)
    .WithFriendlyUrls();

var subscriptionsApi = builder
    .AddProject<Edvantix_Subscriptions>(Services.Subscriptions)
    .WithReference(subscriptionDb)
    .WaitFor(subscriptionDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

var blogApi = builder
    .AddProject<Edvantix_Blog>(Services.Blog)
    .WithReference(blogDb)
    .WaitFor(blogDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(profileApi)
    .WaitFor(profileApi)
    .WithReference(redis)
    .WaitFor(redis)
    .WithFriendlyUrls();

var gateway = builder
    .AddApiGatewayProxy()
    .WithService(organizationApi, true)
    .WithService(profileApi, true)
    .WithService(subscriptionsApi, true)
    .WithService(blogApi, true)
    .Build();

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
    .WithKeycloak(keycloak)
    .WaitFor(gateway);

front.WithEnvironment("NEXT_PUBLIC_APP_URL", front.GetEndpoint(Http.Schemes.Http));

var blogFront = turbo
    .AddApp(Clients.BlogFront, Clients.BlogTurboApp)
    .WithOtlpExporter()
    .WithHttpEndpoint(env: "PORT")
    .WithMappedEndpointPort()
    .WithHttpHealthCheck()
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Http.Schemes.Https))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Http.Schemes.Http))
    .WithKeycloak(keycloak)
    .WaitFor(gateway);

blogFront.WithEnvironment("NEXT_PUBLIC_APP_URL", blogFront.GetEndpoint(Http.Schemes.Http));

if (builder.ExecutionContext.IsRunMode)
{
    builder
        .AddScalar(keycloak)
        .WithOpenAPI(organizationApi)
        .WithOpenAPI(profileApi)
        .WithOpenAPI(subscriptionsApi)
        .WithOpenAPI(blogApi);
}

await builder.Build().RunAsync();
