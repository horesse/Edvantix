using System.Diagnostics;
using System.Text.RegularExpressions;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Edvantix.Chassis.OpenTelemetry;

internal sealed partial class FixHttpRouteProcessor : BaseProcessor<Activity>
{
    private const string HttpRequestMethodTag = "http.request.method";
    private const string UrlPathTag = "url.path";
    private const string HttpRouteTag = "http.route";
    private const string NameTag = "name";
    private const string RequestNameTag = "request.name";

    [GeneratedRegex(@"\{version:apiVersion\}", RegexOptions.IgnoreCase)]
    private static partial Regex ApiVersionTokenRegex();

    [GeneratedRegex(
        @"/v(?<version>\d+(?:\.\d+)?|\d{4}-\d{2}-\d{2})(?=/|$)",
        RegexOptions.IgnoreCase
    )]
    private static partial Regex ApiVersionSegmentRegex();

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

        var existingRoute = activity.GetTagItem(HttpRouteTag)?.ToString();

        if (!string.IsNullOrWhiteSpace(existingRoute))
        {
            if (!ApiVersionTokenRegex().IsMatch(existingRoute))
            {
                return;
            }

            var versionMatch = ApiVersionSegmentRegex().Match(path);
            if (!versionMatch.Success)
            {
                return;
            }

            var resolvedRoute = ApiVersionTokenRegex()
                .Replace(existingRoute, versionMatch.Groups["version"].Value);
            var resolvedDisplayName = $"{method} {resolvedRoute}";

            activity.DisplayName = resolvedDisplayName;
            activity.SetTag(HttpRouteTag, resolvedRoute);
            activity.SetTag(NameTag, resolvedDisplayName);
            activity.SetTag(RequestNameTag, resolvedDisplayName);
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
