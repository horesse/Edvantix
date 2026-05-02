using Edvantix.Notification.Domain.Models;

namespace Edvantix.Notification.IntegrationEvents.EventHandlers;

public static class CleanUpSentEmailIntegrationEventHandler
{
    public static async Task Handle(
        CleanUpSentEmailIntegrationEvent @event,
        ILogger logger,
        GlobalLogBuffer logBuffer,
        IOutboxRepository repository,
        CancellationToken cancellationToken
    )
    {
        try
        {
            logger.LogDebug("Starting cleanup of sent emails...");

            var sentEmails = await repository.ListAsync(new OutboxFilterSpec(), cancellationToken);

            if (sentEmails.Count != 0)
            {
                logger.LogDebug("Found {Count} sent emails to delete", sentEmails.Count);
                repository.BulkDelete(sentEmails);
                await repository.UnitOfWork.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Successfully cleaned up sent emails");
            }
            else
            {
                logger.LogDebug("No sent emails found for cleanup");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while cleaning up sent emails");
            logBuffer.Flush();
            throw new InvalidOperationException("Failed to clean up sent emails", ex);
        }
    }
}
