using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Blog.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория постов блога на основе BlogContext.
/// </summary>
public sealed class PostRepository(IServiceProvider provider)
    : CrudRepository<BlogContext, Post, ulong>(provider),
        IPostRepository;
