using System.Text.RegularExpressions;

namespace Edvantix.Chassis.Utilities.Formatters;

public static class StringFormatter
{
    private static readonly Regex SnakeRegex = new(@"([a-z0-9])([A-Z])", RegexOptions.Compiled);
    
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return value;

        // Разделяем переходы camelCase / PascalCase
        var result = SnakeRegex.Replace(value, "$1_$2");

        // Заменяем все нестандартные разделители на подчеркивания
        result = Regex.Replace(result, @"[\W]+", "_");

        // Убираем повторные подчеркивания
        result = Regex.Replace(result, "_{2,}", "_");

        // Убираем подчеркивание с начала/конца
        result = result.Trim('_');

        return result.ToLowerInvariant();
    }
}
