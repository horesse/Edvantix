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
var auditDb = postgres.AddDatabase(Components.Database.Audit);
var curriculumDb = postgres.AddDatabase(Components.Database.Curriculum);
var scheduleDb = postgres.AddDatabase(Components.Database.Schedule);
var groupsDb = postgres.AddDatabase(Components.Database.Groups);

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
    .WithFriendlyUrls()
    .WithExplicitStart();

var auditApi = builder
    .AddProject<Edvantix_Audit>(Services.Audit)
    .WithKeycloak(keycloak)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(auditDb)
    .WaitFor(auditDb)
    .WithReference(redis)
    .WaitFor(redis)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls()
    .WithExplicitStart();

var curriculumApi = builder
    .AddProject<Edvantix_Curriculum>(Services.Curriculum)
    .WithKeycloak(keycloak)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(curriculumDb)
    .WaitFor(curriculumDb)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

var scheduleApi = builder
    .AddProject<Edvantix_Schedule>(Services.Schedule)
    .WithKeycloak(keycloak)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(scheduleDb)
    .WaitFor(scheduleDb)
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
    .WithReference(curriculumApi)
    .WithContainerRegistry(registry)
    .WithFriendlyUrls();

var groupsApi = builder
    .AddProject<Edvantix_Groups>(Services.Groups)
    .WithKeycloak(keycloak)
    .WithReference(queue)
    .WaitFor(queue)
    .WithReference(groupsDb)
    .WaitFor(groupsDb)
    .WithReference(redis)
    .WaitFor(redis)
    .WithReference(organizationalApi)
    .WithReference(personaApi)
    .WithReference(curriculumApi)
    .WithReference(scheduleApi)
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
    .WithService(auditApi, true)
    .WithService(curriculumApi, true)
    .WithService(scheduleApi, true)
    .WithService(groupsApi, true)
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
        .WithOpenAPI(organizationalApi)
        .WithOpenAPI(auditApi)
        .WithOpenAPI(curriculumApi)
        .WithOpenAPI(scheduleApi)
        .WithOpenAPI(groupsApi);

    builder.AddK6(gateway);
}
else
{
    var (organizationFrontUrl, adminFrontUrl) = builder.AddCorsOriginParameters();

    personaApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    organizationalApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    notificationApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    auditApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    curriculumApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    scheduleApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
    groupsApi.WithCorsOrigins(organizationFrontUrl, adminFrontUrl);
}

await builder.Build().RunAsync();
