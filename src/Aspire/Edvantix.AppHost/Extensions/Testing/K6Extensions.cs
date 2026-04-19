using Aspire.Hosting.Yarp;

namespace Edvantix.AppHost.Extensions.Testing;

internal static class K6Extensions
{
    private const string BaseContainerPath = "Container/k6";
    private const string K6WebDashboard = "K6_WEB_DASHBOARD";
    private const string K6WebDashboardExport = "K6_WEB_DASHBOARD_EXPORT";

    extension(IDistributedApplicationBuilder builder)
    {
        /// <summary>
        /// Добавляет возможность нагрузочного тестирования K6 к распределенным приложениям
        /// с расширенными возможностями тестирования производительности.
        /// </summary>
        /// <param name="entryPoint">Конструктор ресурсов для прокси-ресурса YARP, используемый для тестирования.</param>
        /// <param name="vus">Количество виртуальных пользователей (VU) для имитации во время нагрузочного тестирования. По умолчанию — 10.</param>
        public void AddK6(IResourceBuilder<YarpResource> entryPoint, int vus = 10)
        {
            var scriptPath = Path.GetFullPath($"{BaseContainerPath}", builder.AppHostDirectory);
            var distPath = Path.GetFullPath($"{BaseContainerPath}/dist", builder.AppHostDirectory);

            builder
                .AddK6(Components.K6)
                .WithIconName("BeakerAdd")
                .WithImagePullPolicy(ImagePullPolicy.Always)
                .WithBindMount($"{scriptPath}", "/scripts", true)
                .WithBindMount($"{distPath}", "/home/k6")
                .WithScript("/scripts/dist/main.js", vus)
                .WithReference(entryPoint.Resource.GetEndpoint(Uri.UriSchemeHttp))
                .WithEnvironment(K6WebDashboard, "true")
                .WithEnvironment(K6WebDashboardExport, "dashboard-report.html")
                .WithHttpEndpoint(
                    targetPort: K6DashboardDefaults.ContainerPort,
                    name: K6DashboardDefaults.Name
                )
                .WithUrlForEndpoint(
                    K6DashboardDefaults.Name,
                    url => url.DisplayText = "K6 Dashboard"
                )
                .WithK6OtlpEnvironment()
                .WaitFor(entryPoint)
                .WithExplicitStart();
        }
    }

    private static class K6DashboardDefaults
    {
        public const string Name = "k6-dashboard";
        public const int ContainerPort = 5665;
    }
}
