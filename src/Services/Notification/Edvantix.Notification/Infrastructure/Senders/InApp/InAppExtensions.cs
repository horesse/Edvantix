namespace Edvantix.Notification.Infrastructure.Senders.InApp;

internal static class InAppExtensions
{
    extension(IHostApplicationBuilder builder)
    {
        /// <summary>
        /// Регистрирует <see cref="IInAppSender"/> в контейнере зависимостей.
        /// </summary>
        public void AddInAppSender() =>
            builder.Services.AddScoped<IInAppSender, InAppNotificationSender>();
    }
}
