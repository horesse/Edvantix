using Edvantix.ProfileService.Features.ProfileFeature;

namespace Edvantix.ProfileService.Features;

public static class Extensions
{
    public static void AddApiFeature(this IServiceCollection services)
    {
        // Регистрация только основной фичи профиля
        // Дочерние сущности (Contact, EmploymentHistory, FullName, Education)
        // управляются через Profile aggregate согласно DDD принципам
        services.AddPersonInfoFeature();
    }
}
