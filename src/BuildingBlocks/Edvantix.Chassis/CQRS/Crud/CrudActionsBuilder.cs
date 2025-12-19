using Edvantix.Constants.Other;

namespace Edvantix.Chassis.CQRS.Crud;

/// <summary>
/// Builder для удобной работы с CrudActions
/// </summary>
public static class CrudActionsBuilder
{
    /// <summary>
    /// Создает конфигурацию с только указанными операциями
    /// </summary>
    public static CrudActions Only(params CrudActions[] actions)
    {
        return actions.Aggregate(CrudActions.None, (current, action) => current | action);
    }

    /// <summary>
    /// Создает конфигурацию со всеми операциями, кроме указанных
    /// </summary>
    public static CrudActions Except(params CrudActions[] excludedActions)
    {
        return excludedActions.Aggregate(CrudActions.All, (current, action) => current & ~action);
    }

    extension(CrudActions current)
    {
        /// <summary>
        /// Добавляет операции к существующей конфигурации
        /// </summary>
        public CrudActions With(params CrudActions[] additional)
        {
            return additional.Aggregate(current, (current1, action) => current1 | action);
        }

        /// <summary>
        /// Удаляет операции из существующей конфигурации
        /// </summary>
        public CrudActions Without(params CrudActions[] toRemove)
        {
            return toRemove.Aggregate(current, (current1, action) => current1 & ~action);
        }
    }
}
