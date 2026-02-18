using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;

/// <summary>
/// Репозиторий для работы с подписками пользователей на блог.
/// </summary>
public interface IBlogSubscriptionRepository : ICrudRepository<BlogSubscription, long>;
