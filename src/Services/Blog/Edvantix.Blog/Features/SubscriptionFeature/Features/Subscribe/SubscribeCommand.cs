using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;
using Edvantix.Blog.Grpc.Services;
using MediatR;

namespace Edvantix.Blog.Features.SubscriptionFeature.Features.Subscribe;

/// <summary>
/// Команда для создания или обновления подписки пользователя на блог.
/// Если подписка уже существует — обновляет настройки типов контента.
/// </summary>
public sealed record SubscribeCommand(ContentSubscriptionType ContentTypes) : IRequest<long>;

/// <summary>
/// Обработчик команды подписки.
/// Создаёт новую подписку или обновляет существующую для текущего пользователя.
/// </summary>
public sealed class SubscribeCommandHandler(IServiceProvider provider)
    : IRequestHandler<SubscribeCommand, long>
{
    public async Task<long> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var userId = await provider.GetProfileId(cancellationToken);

        using var subscriptionRepo = provider.GetRequiredService<IBlogSubscriptionRepository>();

        var existingSubscription = await subscriptionRepo.GetOrDefaultAsync(
            s => s.UserId == userId,
            cancellationToken
        );

        if (existingSubscription is not null)
        {
            // Обновляем настройки существующей подписки
            existingSubscription.UpdateContentTypes(request.ContentTypes);
            await subscriptionRepo.UpdateAsync(existingSubscription, cancellationToken);
            await subscriptionRepo.SaveEntitiesAsync(cancellationToken);

            return existingSubscription.Id;
        }

        var subscription = new BlogSubscription(userId, request.ContentTypes);
        await subscriptionRepo.InsertAsync(subscription, cancellationToken);
        await subscriptionRepo.SaveEntitiesAsync(cancellationToken);

        return subscription.Id;
    }
}
