namespace Edvantix.Blog.Features.PostFeature.Models;

/// <summary>
/// Краткая информация об авторе поста для отображения в клиентах.
/// </summary>
public sealed class AuthorModel
{
    /// <summary>Идентификатор профиля автора.</summary>
    public Guid Id { get; set; }

    /// <summary>Полное имя автора.</summary>
    public string FullName { get; set; } = null!;
}
