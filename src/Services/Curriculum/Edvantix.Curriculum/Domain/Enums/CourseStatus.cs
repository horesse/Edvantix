namespace Edvantix.Curriculum.Domain.Enums;

/// <summary>Статус жизненного цикла курса.</summary>
public enum CourseStatus
{
    /// <summary>Черновик — курс в разработке, недоступен для групп.</summary>
    Draft,

    /// <summary>На проверке — курс отправлен на ревью.</summary>
    Review,

    /// <summary>Активен — курс опубликован и доступен для привязки к группам.</summary>
    Active,

    /// <summary>Архивирован — курс больше не используется в новых группах.</summary>
    Archived,
}
