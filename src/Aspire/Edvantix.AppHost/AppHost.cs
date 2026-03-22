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

var queue = builder
    .AddKafka(Components.Broker)
    .WithIconName("Pipeline")
    .WithKafkaUI()
    .WithImagePullPolicy(ImagePullPolicy.Always)
    .WithLifetime(ContainerLifetime.Persistent);

var storage = builder
    .AddAzureStorage(Components.Azure.Storage.Resource)
    .WithIconName("DatabasePlugConnected")
    .RunAsLocalContainer()
    .ProvisionAsService();

var profileContainer = storage
    .AddBlobContainer(Components.Azure.Storage.BlobContainer(Services.Persona))
    .WithAzureStorageExplorer();

var profileDb = postgres.AddDatabase(Components.Database.Persona);
var blogDb = postgres.AddDatabase(Components.Database.Blog);
var notificationDb = postgres.AddDatabase(Components.Database.Notification);

IResourceBuilder<IResource> keycloak = builder.ExecutionContext.IsRunMode
    ? builder.AddLocalKeycloak(Components.KeyCloak)
    : builder.AddHostedKeycloak(Components.KeyCloak);

var personaApi = builder
    .AddProject<Edvantix_Persona>(Services.Persona)
    .WithReference(profileDb)
    .WaitFor(profileDb)
    .WithKeycloak(keycloak)
    .WaitFor(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(profileContainer)
    .WaitFor(profileContainer)
    .WithReference(queue)
    .WaitFor(queue)
    .WithRoleAssignments(
        storage,
        StorageBuiltInRole.StorageBlobDataContributor,
        StorageBuiltInRole.StorageBlobDataOwner
    )
    .WithFriendlyUrls();

var blogApi = builder
    .AddProject<Edvantix_Blog>(Services.Blog)
    .WithReference(blogDb)
    .WaitFor(blogDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(personaApi)
    .WaitFor(personaApi)
    .WithReference(redis)
    .WaitFor(redis)
    .WithFriendlyUrls()
    .WithExplicitStart();

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
    .WithService(personaApi, true)
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
    .WaitFor(gateway)
    .WithExplicitStart();

builder
    .AddProject<Edvantix_Scheduler>(Services.Scheduler)
    .WithReference(queue)
    .WaitFor(queue)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls("Quartz Dashboard", path: Http.Endpoints.QuartzDashboardEndpointPath)
    .WithExplicitStart();

blogFront.WithEnvironment("NEXT_PUBLIC_APP_URL", blogFront.GetEndpoint(Http.Schemes.Http));

var landingFront = turbo
    .AddApp(Clients.LandingFront, Clients.LandingTurboApp)
    .WithOtlpExporter()
    .WithHttpEndpoint(env: "PORT")
    .WithMappedEndpointPort()
    .WithHttpHealthCheck()
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Http.Schemes.Https))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Http.Schemes.Http))
    .WaitFor(gateway)
    .WithExplicitStart();

landingFront.WithEnvironment("NEXT_PUBLIC_APP_URL", landingFront.GetEndpoint(Http.Schemes.Http));

var adminFront = turbo
    .AddApp(Clients.AdminFront, Clients.AdminTurboApp)
    .WithOtlpExporter()
    .WithHttpEndpoint(env: "PORT")
    .WithMappedEndpointPort()
    .WithHttpHealthCheck()
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Http.Schemes.Https))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Http.Schemes.Http))
    .WithKeycloak(keycloak)
    .WaitFor(gateway)
    .WithExplicitStart();

adminFront.WithEnvironment("NEXT_PUBLIC_APP_URL", adminFront.GetEndpoint(Http.Schemes.Http));

if (builder.ExecutionContext.IsRunMode)
{
    builder
        .AddScalar(keycloak)
        .WithOpenAPI(personaApi)
        .WithOpenAPI(blogApi)
        .WithOpenAPI(notificationApi);
}

await builder.Build().RunAsync();
