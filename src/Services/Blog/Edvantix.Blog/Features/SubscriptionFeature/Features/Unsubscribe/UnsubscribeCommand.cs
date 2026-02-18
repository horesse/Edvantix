using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;
using Edvantix.Blog.Grpc.Services;
using Edvantix.Chassis.Exceptions;
using MediatR;

namespace Edvantix.Blog.Features.SubscriptionFeature.Features.Unsubscribe;

/// <summary>
/// Команда для отмены подписки текущего пользователя на блог.
/// </summary>
public sealed record UnsubscribeCommand : IRequest;

/// <summary>
/// Обработчик команды отписки.
/// </summary>
public sealed class UnsubscribeCommandHandler(IServiceProvider provider)
    : IRequestHandler<UnsubscribeCommand>
{
    public async Task Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var userId = await provider.GetProfileId(cancellationToken);

        using var subscriptionRepo = provider.GetRequiredService<IBlogSubscriptionRepository>();

        var subscription =
            await subscriptionRepo.GetOrDefaultAsync(s => s.UserId == userId, cancellationToken)
            ?? throw new NotFoundException("Подписка не найдена.");

        await subscriptionRepo.DeleteAsync(subscription, cancellationToken);
        await subscriptionRepo.SaveEntitiesAsync(cancellationToken);
    }
}
