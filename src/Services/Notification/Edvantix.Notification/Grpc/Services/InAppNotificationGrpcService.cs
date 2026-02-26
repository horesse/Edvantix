using Edvantix.Notification.Domain.Models;
using Edvantix.Notification.Infrastructure;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Edvantix.Notification.Grpc.Services;

/// <summary>
/// gRPC-сервис для межсервисного создания in-app уведомлений.
/// Используется другими микросервисами (Organizational, Persona, Blog и т.д.).
/// Анонимный доступ: аутентификация обеспечивается на уровне сети (сервис-меш / внутренний кластер).
/// </summary>
[AllowAnonymous]
public sealed class InAppNotificationGrpcService(
    InAppNotificationService notificationService,
    ILogger<InAppNotificationGrpcService> logger
) : InAppNotificationGrpcService.InAppNotificationGrpcServiceBase
{
    /// <summary>
    /// Создаёт in-app уведомление для указанного пользователя.
    /// </summary>
    public override async Task<SendInAppNotificationResponse> SendInAppNotification(
        SendInAppNotificationRequest request,
        ServerCallContext context
    )
    {
        if (!Guid.TryParse(request.AccountId, out var accountId))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Некорректный формат account_id.")
            );
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Заголовок уведомления не может быть пустым.")
            );
        }

        if (string.IsNullOrWhiteSpace(request.Message))
        {
            throw new RpcException(
                new Status(StatusCode.InvalidArgument, "Текст уведомления не может быть пустым.")
            );
        }

        try
        {
            var type = (NotificationType)request.Type;
            var metadata = request.HasMetadata ? request.Metadata : null;

            var notification = await notificationService.CreateAsync(
                accountId,
                type,
                request.Title,
                request.Message,
                metadata,
                context.CancellationToken
            );

            return new SendInAppNotificationResponse { NotificationId = notification.Id.ToString() };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "gRPC SendInAppNotification: внутренняя ошибка. AccountId: {AccountId}",
                request.AccountId
            );

            throw new RpcException(new Status(StatusCode.Internal, "Внутренняя ошибка сервера."));
        }
    }
}
