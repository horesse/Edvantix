using System.Diagnostics.CodeAnalysis;

namespace Edvantix.Blog;

/// <summary>
/// Настройки приложения микросервиса Blog.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class AppSettings
{
    /// <summary>
    /// Максимальный размер контента поста в символах.
    /// </summary>
    public int MaxContentLength { get; set; } = 500_000;
}
