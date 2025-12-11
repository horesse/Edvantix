using System.Reflection;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PublicModelAttribute(string? description = null, bool requiredAuth = false) : Attribute
{
    public string Description => description ?? "Неизвестная сущность";
    public bool AuthRequired => requiredAuth;

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
