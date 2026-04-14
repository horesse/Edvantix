using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Edvantix.Chassis.OpenTelemetry;

internal sealed class FixHttpRouteProcessor : BaseProcessor<Activity>
{
    private const string HttpRequestMethodTag = "http.request.method";
    private const string UrlPathTag = "url.path";
    private const string HttpRouteTag = "http.route";
    private const string NameTag = "name";
    private const string RequestNameTag = "request.name";

    public override void OnEnd(Activity activity)
    {
        if (activity.Kind != ActivityKind.Server)
        {
            return;
        }

        var method = activity.GetTagItem(HttpRequestMethodTag)?.ToString();
        var path = activity.GetTagItem(UrlPathTag)?.ToString();

        if (string.IsNullOrWhiteSpace(method) || string.IsNullOrWhiteSpace(path))
        {
            return;
        }

        var displayName = $"{method} {path}";
        activity.DisplayName = displayName;
        activity.SetTag(HttpRouteTag, path);
        activity.SetTag(NameTag, displayName);
        activity.SetTag(RequestNameTag, displayName);
    }
}

public static class FixHttpRouteProcessorExtensions
{
    extension(TracerProviderBuilder tracerProviderBuilder)
    {
        /// <summary>
        /// Добавляет <see cref="FixHttpRouteProcessor" /> в пайплайн <see cref="TracerProviderBuilder" />.
        /// </summary>
        /// <returns>
        /// Экземпляр <see cref="TracerProviderBuilder" /> с зарегистрированным <see cref="FixHttpRouteProcessor" />,
        /// обеспечивающий нормализацию HTTP-маршрутов в трассировках OpenTelemetry.
        /// </returns>
        /// <example>
        /// <code>
        /// builder.Services.AddOpenTelemetry()
        ///     .WithTracing(tracing => tracing.AddFixHttpRouteProcessor());
        /// </code>
        /// </example>
        public TracerProviderBuilder AddFixHttpRouteProcessor()
        {
            return tracerProviderBuilder.AddProcessor(new FixHttpRouteProcessor());
        }
    }
}
