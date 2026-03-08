using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;

namespace Edvantix.Organizational.Infrastructure.Seeders;

/// <summary>
/// Засевает справочные данные в базу данных при первом запуске.
/// В текущей версии заполняет справочник организационно-правовых форм.
/// </summary>
public sealed class OrganizationalDbSeeder(ILogger<OrganizationalDbSeeder> logger)
    : IDbSeeder<OrganizationalDbContext>
{
    public async Task SeedAsync(OrganizationalDbContext context)
    {
        await SeedLegalFormsAsync(context);
    }

    private async Task SeedLegalFormsAsync(OrganizationalDbContext context)
    {
        // Не добавляем дубликаты при повторных запусках.
        if (await context.Set<LegalForm>().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding legal forms...");

        var legalForms = new LegalForm[]
        {
            new("Общество с ограниченной ответственностью", "ООО"),
            new("Открытое акционерное общество", "ОАО"),
            new("Закрытое акционерное общество", "ЗАО"),
            new("Унитарное предприятие", "УП"),
            new("Частное унитарное предприятие", "ЧУП"),
            new("Индивидуальный предприниматель", "ИП"),
            new("Производственный кооператив", "Кооператив"),
            new("Государственное учреждение образования", "ГУО"),
            new("Частное учреждение образования", "ЧУО"),
            new("Учреждение образования", "УО"),
        };

        await context.Set<LegalForm>().AddRangeAsync(legalForms);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} legal forms.", legalForms.Length);
    }
}
