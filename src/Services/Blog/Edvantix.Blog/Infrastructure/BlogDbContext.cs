using Edvantix.Chassis.EF.Contexts;

namespace Edvantix.Blog.Infrastructure;

/// <summary>
/// Контекст базы данных микросервиса Blog.
/// Управляет постами, категориями, тегами, лайками и подписками.
/// </summary>
public sealed class BlogDbContext(DbContextOptions options) : PostgresContext(options);
