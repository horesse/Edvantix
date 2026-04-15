using System.Globalization;
using System.Threading.RateLimiting;
using Edvantix.Chassis.Utilities.Configurations;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.RateLimiting;

namespace Edvantix.ServiceDefaults.Kestrel;

public static class RateLimiterExtensions
{
    private const string PerUserPolicy = "PerUserRateLimit";

    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует и настраивает ограничение частоты запросов API для текущего хоста.
        /// </summary>
        /// <remarks>
        /// Метод настраивает:
        /// <list type="bullet">
        ///     <item><description>Глобальный ограничитель фиксированного окна для всех запросов.</description></item>
        ///     <item><description>Политику токенового ведра на пользователя с именем <c>PerUserRateLimit</c>.</description></item>
        ///     <item><description>Единое поведение отклонения, возвращающее HTTP <c>429</c>.</description></item>
        /// </list>
        /// </remarks>
        public void AddRateLimiting()
        {
            var services = builder.Services;

            // Регистрирует сервисы ограничения частоты запросов ASP.NET Core.
            services.AddRateLimiter();

            // Настраивает параметры глобального ограничителя фиксированного окна.
            builder.Configure<FixedWindowRateLimiterOptions>(
                $"{nameof(RateLimiter)}:{nameof(FixedWindowRateLimiter)}",
                configure: options =>
                {
                    options.AutoReplenishment = true;
                    options.PermitLimit = 30;
                    options.QueueLimit = 0;
                    options.Window = TimeSpan.FromMinutes(1);
                }
            );

            // Настраивает параметры токенового ведра для ограничения на пользователя.
            builder.Configure<TokenBucketRateLimiterOptions>(
                $"{nameof(RateLimiter)}:{nameof(TokenBucketRateLimiter)}",
                configure: options =>
                {
                    options.AutoReplenishment = true;
                    options.TokenLimit = 30;
                    options.TokensPerPeriod = 20;
                    options.QueueLimit = 5;
                    options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                }
            );

            // Применяет поведение конвейера ограничителя и привязывает политики.
            services
                .AddOptions<RateLimiterOptions>()
                .Configure(
                    (
                        RateLimiterOptions options,
                        IOptionsMonitor<TokenBucketRateLimiterOptions> userRateLimitingOptions,
                        IOptionsMonitor<FixedWindowRateLimiterOptions> windowRateLimiterOptions
                    ) =>
                    {
                        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                        options.AddRejectBehavior();

                        options.AddDefaultLimiter(windowRateLimiterOptions);

                        options.AddUserRateLimiter(userRateLimitingOptions);
                    }
                );
        }
    }

    extension(IEndpointConventionBuilder builder)
    {
        /// <summary>
        /// Применяет политику ограничения частоты запросов на пользователя к эндпоинту.
        /// </summary>
        /// <returns>Построитель соглашений эндпоинта с применённой политикой ограничения.</returns>
        public IEndpointConventionBuilder RequirePerUserRateLimit()
        {
            return builder.RequireRateLimiting(PerUserPolicy);
        }
    }

    extension(RateLimiterOptions option)
    {
        private void AddDefaultLimiter(
            IOptionsMonitor<FixedWindowRateLimiterOptions> fixedWindowRateLimiterOptions
        )
        {
            option.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    context.Connection.RemoteIpAddress?.ToString()
                        ?? context.Request.Headers.Host.ToString(),
                    _ => fixedWindowRateLimiterOptions.CurrentValue
                )
            );
        }

        private void AddUserRateLimiter(
            IOptionsMonitor<TokenBucketRateLimiterOptions> userRateLimitingOptions
        )
        {
            option.AddPolicy(
                PerUserPolicy,
                context =>
                {
                    var username = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

                    if (string.IsNullOrEmpty(username))
                    {
                        throw new UnauthorizedAccessException(
                            "User is not authenticated. Rate limiting requires an authenticated user."
                        );
                    }

                    return RateLimitPartition.GetTokenBucketLimiter(
                        username,
                        _ => userRateLimitingOptions.CurrentValue
                    );
                }
            );
        }

        private void AddRejectBehavior()
        {
            option.OnRejected = async (context, cancellationToken) =>
            {
                var httpContext = context.HttpContext;
                var serviceProvider = httpContext.RequestServices;

                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    var milliseconds = retryAfter.TotalMilliseconds.ToString(
                        CultureInfo.InvariantCulture
                    );

                    httpContext.Response.Headers.RetryAfter = milliseconds;

                    var problemDetailsFactory =
                        serviceProvider.GetRequiredService<ProblemDetailsFactory>();

                    var problemDetails = problemDetailsFactory.CreateProblemDetails(
                        httpContext,
                        StatusCodes.Status429TooManyRequests,
                        "Rate limit exceeded",
                        $"You have exceeded the rate limit. Try again in {milliseconds} ms."
                    );

                    await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
                }
            };
        }
    }
}
