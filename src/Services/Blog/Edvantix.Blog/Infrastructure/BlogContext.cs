using Edvantix.Chassis.EF.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Edvantix.Blog.Infrastructure;

/// <summary>
/// Контекст базы данных микросервиса Blog.
/// Управляет постами, категориями, тегами, лайками и подписками.
/// </summary>
public sealed class BlogContext(DbContextOptions options) : PostgresContext(options);
