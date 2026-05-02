using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Edvantix.Chassis.Utilities;

/// <summary>Вспомогательные методы для работы с enum-разрешениями.</summary>
public static class EnumExtensions
{
    extension(Enum value)
    {
        /// <summary>
        /// Возвращает машинный код — строковое имя члена enum.
        /// Соответствует значению, хранящемуся в базе данных.
        /// </summary>
        public string GetCode() => value.ToString();

        /// <summary>
        /// Возвращает человекочитаемое название из атрибута <see cref="DisplayAttribute"/>.
        /// Если атрибут не задан, возвращает имя члена enum.
        /// </summary>
        public string GetDisplayName()
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var display = member?.GetCustomAttribute<DisplayAttribute>();
            return display?.Name ?? value.ToString();
        }
    }

    extension(Type enumType)
    {
        /// <summary>
        /// Возвращает человекочитаемое название из атрибута
        /// <see cref="DescriptionAttribute"/> на типе enum.
        /// Если атрибут не задан, возвращает имя типа.
        /// </summary>
        public string GetDisplayName()
        {
            var desc = enumType.GetCustomAttribute<DescriptionAttribute>();
            return desc?.Description ?? enumType.Name;
        }
    }
}
