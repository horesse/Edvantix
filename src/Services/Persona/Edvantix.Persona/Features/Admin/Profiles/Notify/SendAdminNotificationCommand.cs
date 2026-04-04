using Edvantix.Constants.Other;
using Edvantix.Contracts;

namespace Edvantix.Persona.Features.Admin.Profiles.Notify;

public sealed record SendAdminNotificationRequest(
    string Title,
    string Message,
    NotificationType Type = NotificationType.Info
);

public sealed record SendAdminNotificationCommand(
    Guid ProfileId,
    string Title,
    string Message,
    NotificationType Type
) : ICommand;

public sealed class SendAdminNotificationCommandHandler(
    IProfileRepository repository,
    IBus bus,
    ILogger<SendAdminNotificationCommandHandler> logger
) : ICommandHandler<SendAdminNotificationCommand>
{
    public async ValueTask<Unit> Handle(
        SendAdminNotificationCommand request,
        CancellationToken cancellationToken
    )
    {
        var spec = ProfileSpecification.Minimal(request.ProfileId);
        var profile = await repository.FindAsync(spec, cancellationToken);

        Guard.Against.NotFound(profile, request.ProfileId);

        var integrationEvent = new SendInAppNotificationIntegrationEvent(
            profile.AccountId,
            request.Type,
            request.Title,
            request.Message
        );

        await bus.Publish(integrationEvent, cancellationToken);

        logger.LogInformation(
            "Уведомление отправлено профилю {ProfileId} (аккаунт {AccountId}) администратором",
            request.ProfileId,
            profile.AccountId
        );

        return Unit.Value;
    }
}
