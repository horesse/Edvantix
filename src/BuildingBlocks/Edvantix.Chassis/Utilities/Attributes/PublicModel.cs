using System.Reflection;
using Edvantix.Constants.Other;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PublicModelAttribute(
    string? desc = null,
    EntityGroupEnum entityType = EntityGroupEnum.Hidden,
    bool requiredAuth = false
) : Attribute
{
    public string Description => desc ?? "Неизвестная сущность";
    public bool AuthRequired => requiredAuth;

    public EntityGroupEnum EntityType => entityType;

    // TODO: Разобраться
    public string? AuthPolicy => null;
}

public static class PublicModelAttributeHelper
{
    extension(Type type)
    {
        public bool IsPublicModel()
        {
            var attribute = type.GetCustomAttribute<PublicModelAttribute>();

            return attribute != null;
        }

        public EntityGroupEnum GetEntityType()
        {
            var attribute = type.GetCustomAttribute<PublicModelAttribute>();

            return attribute?.EntityType ?? EntityGroupEnum.Hidden;
        }

        public string GetDescription()
        {
            var attribute = type.GetCustomAttribute<PublicModelAttribute>();

            return attribute?.Description ?? "Неизвестная сущность";
        }

        public bool IsAuthRequired()
        {
            var attribute = type.GetCustomAttribute<PublicModelAttribute>();

            return attribute?.AuthRequired ?? true;
        }

        public string? GetPolicy()
        {
            var attribute = type.GetCustomAttribute<PublicModelAttribute>();

            return attribute?.AuthPolicy;
        }
    }
}
