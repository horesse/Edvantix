using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория лайков постов на основе BlogContext.
/// </summary>
public sealed class PostLikeRepository(IServiceProvider provider)
    : CrudRepository<BlogContext, PostLike, ulong>(provider),
        IPostLikeRepository;
