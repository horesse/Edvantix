using Edvantix.Organizational.Pipelines;

namespace Edvantix.Organizational.Extensions;

public static class MediatorExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Регистрирует <see cref="IPermissionChecker"/> и
        /// <see cref="AuthorizationBehavior{TMessage,TResponse}"/> в конвейере Mediator.
        /// Проверяет разрешения профиля в организации для команд и запросов с атрибутом <c>[RequirePermission]</c>.
        /// </summary>
        internal IServiceCollection ApplyAuthorizationBehavior()
        {
            services.AddScoped<IPermissionChecker, PermissionChecker>();
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
            return services;
        }
    }
}
