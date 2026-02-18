using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;
using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория подписок на блог на основе BlogContext.
/// </summary>
public sealed class BlogSubscriptionRepository(IServiceProvider provider)
    : CrudRepository<BlogContext, BlogSubscription, long>(provider),
        IBlogSubscriptionRepository;
