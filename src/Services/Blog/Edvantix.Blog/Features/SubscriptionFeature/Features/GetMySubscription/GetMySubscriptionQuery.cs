using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;
using Edvantix.Blog.Features.SubscriptionFeature.Models;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.SubscriptionFeature.Features.GetMySubscription;

/// <summary>
/// Запрос для получения настроек подписки текущего пользователя.
/// </summary>
public sealed record GetMySubscriptionQuery : IRequest<SubscriptionModel>;

/// <summary>
/// Обработчик запроса на получение настроек подписки.
/// </summary>
public sealed class GetMySubscriptionQueryHandler(IServiceProvider provider)
    : IRequestHandler<GetMySubscriptionQuery, SubscriptionModel>
{
    public async Task<SubscriptionModel> Handle(
        GetMySubscriptionQuery request,
        CancellationToken cancellationToken
    )
    {
        var userId = await provider.GetProfileId(cancellationToken);

        using var subscriptionRepo = provider.GetRequiredService<IBlogSubscriptionRepository>();

        var subscription =
            await subscriptionRepo.GetOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Подписка на блог не найдена.");

        return new SubscriptionModel
        {
            Id = subscription.Id,
            UserId = subscription.UserId,
            ContentTypes = subscription.ContentTypes,
            CreatedAt = subscription.CreatedAt,
            UpdatedAt = subscription.UpdatedAt,
        };
    }
}
