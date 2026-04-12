using Microsoft.Extensions.Hosting;

namespace Edvantix.Chassis.Utilities.Configurations;

public static class LaunchProfileExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Определяет, использует ли текущий профиль запуска схему HTTPS.
        /// </summary>
        /// <returns>
        /// <see langword="true" />, если переменная <c>DOTNET_LAUNCH_PROFILE</c> равна <see cref="Uri.UriSchemeHttps" />; иначе
        /// <see langword="false" />.
        /// </returns>
        public bool IsHttpsLaunchProfile()
        {
            return builder.Configuration["DOTNET_LAUNCH_PROFILE"] == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Возвращает схему URL для текущего профиля запуска.
        /// </summary>
        /// <returns><see cref="Uri.UriSchemeHttps" /> для HTTPS-профилей; иначе <see cref="Uri.UriSchemeHttp" />.</returns>
        public string GetScheme()
        {
            return builder.IsHttpsLaunchProfile() ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
        }
    }
}
