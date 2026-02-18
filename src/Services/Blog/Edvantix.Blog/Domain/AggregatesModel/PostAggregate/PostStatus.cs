namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Статус жизненного цикла поста в блоге.
/// </summary>
public enum PostStatus
{
    /// <summary>Черновик — пост не опубликован и виден только администраторам.</summary>
    Draft = 0,

    /// <summary>Запланирован — пост будет опубликован в указанное время.</summary>
    Scheduled = 1,

    /// <summary>Опубликован — пост доступен пользователям.</summary>
    Published = 2,

    /// <summary>В архиве — пост скрыт из публичного списка.</summary>
    Archived = 3,
}
