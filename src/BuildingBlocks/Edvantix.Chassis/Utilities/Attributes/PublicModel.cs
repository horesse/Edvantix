using System.Reflection;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PublicModel(string? description = null) : Attribute
{
    public string Description => description ?? "Неизвестная сущность";
}

public static class PublicModelAttributeHelper
{
    extension(Type type)
    {
        public bool IsPublicModel()
        {
            var attribute = type.GetCustomAttribute<PublicModel>();

            return attribute != null;
        }

        public string GetDescription()
        {
            var attribute = type.GetCustomAttribute<PublicModel>();

            return attribute?.Description ?? "Неизвестная сущность";
        }
    }
}
