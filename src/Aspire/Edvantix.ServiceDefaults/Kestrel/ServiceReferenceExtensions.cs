using Grpc.Core;
using Grpc.Health.V1;
using Polly.Timeout;
using Refit;

namespace Edvantix.ServiceDefaults.Kestrel;

public static class ServiceReferenceExtensions
{
    private static readonly string _healthCheckName = nameof(Health).ToLowerInvariant();

    private static void AddGrpcHealthChecks(
        IServiceCollection services,
        Uri uri,
        string healthCheckName,
        HealthStatus failureStatus = default
    )
    {
        services
            .AddGrpcClient<Health.HealthClient>(o => o.Address = uri)
            .AddStandardResilienceHandler();
        services.AddHealthChecks().AddCheck<GrpcServiceHealthCheck>(healthCheckName, failureStatus);
    }

    extension(IServiceCollection services)
    {
        /// <summary>
        /// Добавляет типизированный gRPC-клиент с устойчивостью и регистрацией проверки работоспособности.
        /// </summary>
        /// <typeparam name="TClient">
        /// Контракт типизированного gRPC-клиента.
        /// </typeparam>
        /// <param name="address">
        /// Абсолютный URI нижестоящего gRPC-сервиса.
        /// </param>
        /// <param name="failureStatus">
        /// Одно из значений перечисления, определяющее статус работоспособности при сбое пробы.
        /// </param>
        /// <returns>
        /// Построитель HTTP-клиента для зарегистрированного gRPC-клиента.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Адрес не является допустимым абсолютным URI.
        /// </exception>
        public IHttpClientBuilder AddGrpcServiceReference<TClient>(
            string address,
            HealthStatus failureStatus
        )
            where TClient : class
        {
            if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
            {
                throw new ArgumentException(
                    "Address must be a valid absolute URI.",
                    nameof(address)
                );
            }

            var uri = new Uri(address);
            var builder = services.AddGrpcClient<TClient>(o => o.Address = uri);

            builder.AddStandardResilienceHandler();

            AddGrpcHealthChecks(
                services,
                uri,
                $"{typeof(TClient).Name}-{_healthCheckName}",
                failureStatus
            );

            return builder;
        }

        /// <summary>
        /// Добавляет типизированный HTTP-клиент с необязательной регистрацией проверки работоспособности эндпоинта.
        /// </summary>
        /// <typeparam name="TClient">
        /// Контракт типизированного Refit-клиента.
        /// </typeparam>
        /// <param name="address">
        /// Абсолютный URI нижестоящего HTTP-сервиса.
        /// </param>
        /// <param name="failureStatus">
        /// Одно из значений перечисления, определяющее статус работоспособности при сбое пробы.
        /// </param>
        /// <param name="healthRelativePath">
        /// Относительный путь URI эндпоинта проверки работоспособности. Если <see langword="null" /> или пустой, используется путь по умолчанию.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Адрес не является допустимым абсолютным URI.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Путь проверки работоспособности не является допустимым относительным URI.
        /// </exception>
        public void AddHttpServiceReference<TClient>(
            string address,
            HealthStatus failureStatus,
            string? healthRelativePath = null
        )
            where TClient : class
        {
            if (!Uri.IsWellFormedUriString(address, UriKind.Absolute))
            {
                throw new ArgumentException(
                    "Address must be a valid absolute URI.",
                    nameof(address)
                );
            }

            if (
                !string.IsNullOrEmpty(healthRelativePath)
                && !Uri.IsWellFormedUriString(healthRelativePath, UriKind.Relative)
            )
            {
                throw new ArgumentException(
                    "Health check path must be a valid relative URI.",
                    nameof(healthRelativePath)
                );
            }

            var uri = new Uri(address);

            services.AddRefitClient<TClient>().ConfigureHttpClient(c => c.BaseAddress = uri);

            services
                .AddHealthChecks()
                .AddUrlGroup(
                    new Uri(uri, healthRelativePath ?? _healthCheckName),
                    $"{typeof(TClient).Name}-{_healthCheckName}",
                    failureStatus,
                    configurePrimaryHttpMessageHandler: s =>
                        s.GetRequiredService<IHttpMessageHandlerFactory>().CreateHandler()
                );
        }
    }

    private sealed class GrpcServiceHealthCheck(Health.HealthClient healthClient) : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default
        )
        {
            try
            {
                var response = await healthClient.CheckAsync(
                    new HealthCheckRequest(),
                    cancellationToken: cancellationToken
                );

                return response.Status switch
                {
                    HealthCheckResponse.Types.ServingStatus.Serving => HealthCheckResult.Healthy(),
                    _ => HealthCheckResult.Unhealthy(),
                };
            }
            catch (RpcException exception)
                when (exception.StatusCode is StatusCode.Cancelled or StatusCode.DeadlineExceeded)
            {
                return HealthCheckResult.Unhealthy(exception.Status.Detail, exception);
            }
            catch (TimeoutRejectedException exception)
            {
                return HealthCheckResult.Unhealthy(exception.Message, exception);
            }
        }
    }
}
