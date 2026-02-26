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

var profileContainer = storage
    .AddBlobContainer(Components.Azure.Storage.BlobContainer(Services.Persona))
    .WithAzureStorageExplorer();

var organizationDb = postgres.AddDatabase(Components.Database.Organizational);
var profileDb = postgres.AddDatabase(Components.Database.Persona);
var subscriptionDb = postgres.AddDatabase(Components.Database.Subscription);
var blogDb = postgres.AddDatabase(Components.Database.Blog);
var notificationDb = postgres.AddDatabase(Components.Database.Notification);

IResourceBuilder<IResource> keycloak = builder.ExecutionContext.IsRunMode
    ? builder.AddLocalKeycloak(Components.KeyCloak)
    : builder.AddHostedKeycloak(Components.KeyCloak);

var profileApi = builder
    .AddProject<Edvantix_Persona>(Services.Persona)
    .WithReference(profileDb)
    .WaitFor(profileDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(profileContainer)
    .WaitFor(profileContainer)
    .WithRoleAssignments(
        storage,
        StorageBuiltInRole.StorageBlobDataContributor,
        StorageBuiltInRole.StorageBlobDataOwner
    )
    .WithFriendlyUrls();

var organizationalApi = builder
    .AddProject<Edvantix_Organizational>(Services.Organizational)
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

var notificationApi = builder
    .AddProject<Edvantix_Notification>(Services.Notification)
    .WithEmailProvider()
    .WithKeycloak(keycloak)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(notificationDb)
    .WaitFor(notificationDb)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

var gateway = builder
    .AddApiGatewayProxy()
    .WithService(organizationalApi, true)
    .WithService(profileApi, true)
    .WithService(subscriptionsApi, true)
    .WithService(blogApi, true)
    .WithService(notificationApi, true)
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

builder
    .AddProject<Edvantix_Scheduler>(Services.Scheduler)
    .WithReference(queue)
    .WaitFor(queue)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls(path: Http.Endpoints.AlivenessEndpointPath)
    .WithExplicitStart();

blogFront.WithEnvironment("NEXT_PUBLIC_APP_URL", blogFront.GetEndpoint(Http.Schemes.Http));

if (builder.ExecutionContext.IsRunMode)
{
    builder
        .AddScalar(keycloak)
        .WithOpenAPI(organizationalApi)
        .WithOpenAPI(profileApi)
        .WithOpenAPI(subscriptionsApi)
        .WithOpenAPI(blogApi)
        .WithOpenAPI(notificationApi);
}

await builder.Build().RunAsync();
