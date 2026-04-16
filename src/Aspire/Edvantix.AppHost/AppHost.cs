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
var identityDb = postgres.AddDatabase(Components.Database.Identity);
var notificationDb = postgres.AddDatabase(Components.Database.Notification);
var organizationalDb = postgres.AddDatabase(Components.Database.Organizational);

IResourceBuilder<IResource> keycloak = builder.ExecutionContext.IsRunMode
    ? builder.AddLocalKeycloak(Components.KeyCloak)
    : builder.AddHostedKeycloak(Components.KeyCloak);

var personaApi = builder
    .AddProject<Edvantix_Persona>(Services.Persona)
    .WithReference(profileDb)
    .WaitFor(profileDb)
    .WithKeycloak(keycloak)
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

var organizationalApi = builder
    .AddProject<Edvantix_Organizational>(Services.Organisational)
    .WithKeycloak(keycloak)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(organizationalDb)
    .WaitFor(organizationalDb)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(personaApi)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

builder
    .AddProject<Edvantix_Identity>(Services.Identity)
    .WithReference(identityDb)
    .WaitFor(identityDb)
    .WithKeycloak(keycloak)
    .WithContainerRegistry(registry)
    .WithReference(queue)
    .WaitFor(queue);

var gateway = builder
    .AddApiGatewayProxy()
    .WithService(personaApi, true)
    .WithService(notificationApi, true)
    .WithService(organizationalApi, true)
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
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Uri.UriSchemeHttps))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Uri.UriSchemeHttp))
    .WithKeycloak(keycloak)
    .WaitFor(gateway);

front.WithEnvironment("NEXT_PUBLIC_APP_URL", front.GetEndpoint(Uri.UriSchemeHttp));

builder
    .AddProject<Edvantix_Scheduler>(Services.Scheduler)
    .WithReference(queue)
    .WaitFor(queue)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls("Quartz Dashboard", path: Http.Endpoints.QuartzDashboardEndpointPath)
    .WithExplicitStart();

var landingFront = turbo
    .AddApp(Clients.LandingFront, Clients.LandingTurboApp)
    .WithOtlpExporter()
    .WithHttpEndpoint(env: "PORT")
    .WithMappedEndpointPort()
    .WithHttpHealthCheck()
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Uri.UriSchemeHttps))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Uri.UriSchemeHttp))
    .WaitFor(gateway)
    .WithExplicitStart();

landingFront.WithEnvironment("NEXT_PUBLIC_APP_URL", landingFront.GetEndpoint(Uri.UriSchemeHttp));

var adminFront = turbo
    .AddApp(Clients.AdminFront, Clients.AdminTurboApp)
    .WithOtlpExporter()
    .WithHttpEndpoint(env: "PORT")
    .WithMappedEndpointPort()
    .WithHttpHealthCheck()
    .WithExternalHttpEndpoints()
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTPS", gateway.GetEndpoint(Uri.UriSchemeHttps))
    .WithEnvironment("NEXT_PUBLIC_GATEWAY_HTTP", gateway.GetEndpoint(Uri.UriSchemeHttp))
    .WithKeycloak(keycloak)
    .WaitFor(gateway)
    .WithExplicitStart();

adminFront.WithEnvironment("NEXT_PUBLIC_APP_URL", adminFront.GetEndpoint(Uri.UriSchemeHttp));

if (builder.ExecutionContext.IsRunMode)
{
    builder
        .AddScalar(keycloak)
        .WithOpenAPI(personaApi)
        .WithOpenAPI(notificationApi)
        .WithOpenAPI(organizationalApi);
}
else
{
    var (organizationFrontUrl, adminFrontUrl) = builder.AddCorsOriginParameters();

    personaApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    organizationalApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    notificationApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
}

await builder.Build().RunAsync();
