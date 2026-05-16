using System.Runtime.CompilerServices;

namespace Edvantix.Organizational.UnitTests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        // Verify хранит snapshot-файлы (*.verified.txt) рядом с тест-классами по умолчанию.
        // Дополнительная настройка добавляется здесь по мере необходимости.
        VerifierSettings.InitializePlugins();
    }
}
