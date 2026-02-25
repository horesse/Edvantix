using SendGrid;

namespace Edvantix.Notification.Infrastructure.Senders.SendGrid;

internal sealed class InjectableSendGridClient(
    HttpClient httpClient,
    IOptions<SendGridClientOptions> options
) : BaseClient(httpClient, options.Value);
