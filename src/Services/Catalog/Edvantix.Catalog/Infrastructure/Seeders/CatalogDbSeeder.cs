using Edvantix.Catalog.Domain.AggregatesModel.CountryAggregate;
using Edvantix.Catalog.Domain.AggregatesModel.CurrencyAggregate;
using Edvantix.Catalog.Domain.AggregatesModel.LanguageAggregate;
using Edvantix.Catalog.Domain.AggregatesModel.RegionAggregate;
using Edvantix.Catalog.Domain.AggregatesModel.TimezoneAggregate;
using Edvantix.Chassis.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Edvantix.Catalog.Infrastructure.Seeders;

/// <summary>
/// Засевает начальные справочные данные каталога: валюты, страны, часовые пояса, языки, регионы.
/// Идемпотентен — повторный запуск не создаёт дублей.
/// </summary>
public sealed class CatalogDbSeeder(ILogger<CatalogDbSeeder> logger) : IDbSeeder<CatalogDbContext>
{
    public async Task SeedAsync(CatalogDbContext context)
    {
        // Порядок важен: сначала валюты, затем страны (FK), затем остальные
        await SeedCurrenciesAsync(context);
        await SeedCountriesAsync(context);
        await SeedTimezonesAsync(context);
        await SeedLanguagesAsync(context);
        await SeedRegionsAsync(context);
    }

    private async Task SeedCurrenciesAsync(CatalogDbContext context)
    {
        if (await context.Set<Currency>().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding currencies...");

        var currencies = new Currency[]
        {
            new("USD", "Доллар США", "US Dollar", "$", 840, 2),
            new("EUR", "Евро", "Euro", "€", 978, 2),
            new("GBP", "Фунт стерлингов", "Pound Sterling", "£", 826, 2),
            new("CHF", "Швейцарский франк", "Swiss Franc", "Fr", 756, 2),
            new("JPY", "Японская иена", "Japanese Yen", "¥", 392, 0),
            new("CNY", "Китайский юань", "Chinese Yuan", "¥", 156, 2),
            new("RUB", "Российский рубль", "Russian Ruble", "₽", 643, 2),
            new("BYN", "Белорусский рубль", "Belarusian Ruble", "Br", 933, 2),
            new("KZT", "Казахстанский тенге", "Kazakhstani Tenge", "₸", 398, 2),
            new("UAH", "Украинская гривна", "Ukrainian Hryvnia", "₴", 980, 2),
            new("UZS", "Узбекский сум", "Uzbekistani Sum", "сум", 860, 2),
            new("AZN", "Азербайджанский манат", "Azerbaijani Manat", "₼", 944, 2),
            new("GEL", "Грузинский лари", "Georgian Lari", "₾", 981, 2),
            new("AMD", "Армянский драм", "Armenian Dram", "֏", 051, 2),
            new("MDL", "Молдавский лей", "Moldovan Leu", "L", 498, 2),
            new("KGS", "Киргизский сом", "Kyrgyzstani Som", "с", 417, 2),
            new("TJS", "Таджикский сомони", "Tajikistani Somoni", "SM", 972, 2),
            new("TMT", "Туркменский манат", "Turkmenistani Manat", "T", 934, 2),
            new("TRY", "Турецкая лира", "Turkish Lira", "₺", 949, 2),
            new("AED", "Дирхам ОАЭ", "UAE Dirham", "د.إ", 784, 2),
            new("SAR", "Саудовский риял", "Saudi Riyal", "﷼", 682, 2),
            new("INR", "Индийская рупия", "Indian Rupee", "₹", 356, 2),
            new("KRW", "Южнокорейская вона", "South Korean Won", "₩", 410, 0),
            new("PLN", "Польский злотый", "Polish Zloty", "zł", 985, 2),
            new("CZK", "Чешская крона", "Czech Koruna", "Kč", 203, 2),
        };

        await context.Set<Currency>().AddRangeAsync(currencies);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} currencies.", currencies.Length);
    }

    private async Task SeedCountriesAsync(CatalogDbContext context)
    {
        if (await context.Set<Country>().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding countries...");

        var countries = new Country[]
        {
            // СНГ и страны-партнёры
            new("RU", "RUS", "Россия", "Russia", 643, "+7", "RUB"),
            new("BY", "BLR", "Беларусь", "Belarus", 112, "+375", "BYN"),
            new("KZ", "KAZ", "Казахстан", "Kazakhstan", 398, "+7", "KZT"),
            new("UZ", "UZB", "Узбекистан", "Uzbekistan", 860, "+998", "UZS"),
            new("UA", "UKR", "Украина", "Ukraine", 804, "+380", "UAH"),
            new("AZ", "AZE", "Азербайджан", "Azerbaijan", 031, "+994", "AZN"),
            new("GE", "GEO", "Грузия", "Georgia", 268, "+995", "GEL"),
            new("AM", "ARM", "Армения", "Armenia", 051, "+374", "AMD"),
            new("MD", "MDA", "Молдова", "Moldova", 498, "+373", "MDL"),
            new("KG", "KGZ", "Кыргызстан", "Kyrgyzstan", 417, "+996", "KGS"),
            new("TJ", "TJK", "Таджикистан", "Tajikistan", 762, "+992", "TJS"),
            new("TM", "TKM", "Туркменистан", "Turkmenistan", 795, "+993", "TMT"),
            // Европа
            new("DE", "DEU", "Германия", "Germany", 276, "+49", "EUR"),
            new("FR", "FRA", "Франция", "France", 250, "+33", "EUR"),
            new("GB", "GBR", "Великобритания", "United Kingdom", 826, "+44", "GBP"),
            new("PL", "POL", "Польша", "Poland", 616, "+48", "PLN"),
            new("CZ", "CZE", "Чехия", "Czech Republic", 203, "+420", "CZK"),
            new("NL", "NLD", "Нидерланды", "Netherlands", 528, "+31", "EUR"),
            new("CH", "CHE", "Швейцария", "Switzerland", 756, "+41", "CHF"),
            // Азия и Ближний Восток
            new("TR", "TUR", "Турция", "Turkey", 792, "+90", "TRY"),
            new("AE", "ARE", "ОАЭ", "United Arab Emirates", 784, "+971", "AED"),
            new("SA", "SAU", "Саудовская Аравия", "Saudi Arabia", 682, "+966", "SAR"),
            new("IN", "IND", "Индия", "India", 356, "+91", "INR"),
            new("CN", "CHN", "Китай", "China", 156, "+86", "CNY"),
            new("KR", "KOR", "Республика Корея", "South Korea", 410, "+82", "KRW"),
            new("JP", "JPN", "Япония", "Japan", 392, "+81", "JPY"),
            // Америка
            new("US", "USA", "Соединённые Штаты Америки", "United States", 840, "+1", "USD"),
        };

        await context.Set<Country>().AddRangeAsync(countries);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} countries.", countries.Length);
    }

    private async Task SeedTimezonesAsync(CatalogDbContext context)
    {
        if (await context.Set<Timezone>().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding timezones...");

        var timezones = new Timezone[]
        {
            new(
                "Europe/Kaliningrad",
                "Калининградское время",
                "Kaliningrad Time",
                "(UTC+02:00) Калининград",
                120
            ),
            new(
                "Europe/Moscow",
                "Московское время",
                "Moscow Time",
                "(UTC+03:00) Москва, Санкт-Петербург",
                180
            ),
            new(
                "Europe/Samara",
                "Самарское время",
                "Samara Time",
                "(UTC+04:00) Самара, Удмуртия",
                240
            ),
            new(
                "Asia/Yekaterinburg",
                "Екатеринбургское время",
                "Yekaterinburg Time",
                "(UTC+05:00) Екатеринбург",
                300
            ),
            new("Asia/Omsk", "Омское время", "Omsk Time", "(UTC+06:00) Омск", 360),
            new(
                "Asia/Krasnoyarsk",
                "Красноярское время",
                "Krasnoyarsk Time",
                "(UTC+07:00) Красноярск, Новосибирск",
                420
            ),
            new("Asia/Irkutsk", "Иркутское время", "Irkutsk Time", "(UTC+08:00) Иркутск", 480),
            new("Asia/Yakutsk", "Якутское время", "Yakutsk Time", "(UTC+09:00) Якутск", 540),
            new(
                "Asia/Vladivostok",
                "Владивостокское время",
                "Vladivostok Time",
                "(UTC+10:00) Владивосток",
                600
            ),
            new("Asia/Magadan", "Магаданское время", "Magadan Time", "(UTC+11:00) Магадан", 660),
            new(
                "Asia/Kamchatka",
                "Камчатское время",
                "Kamchatka Time",
                "(UTC+12:00) Камчатка",
                720
            ),
            new("Europe/Minsk", "Беларусь", "Belarus Time", "(UTC+03:00) Минск", 180),
            new(
                "Asia/Almaty",
                "Алматинское время",
                "Almaty Time",
                "(UTC+06:00) Алматы, Нур-Султан",
                360
            ),
            new("Asia/Tashkent", "Ташкентское время", "Tashkent Time", "(UTC+05:00) Ташкент", 300),
            new("Asia/Baku", "Бакинское время", "Baku Time", "(UTC+04:00) Баку", 240),
            new("Asia/Tbilisi", "Тбилисское время", "Tbilisi Time", "(UTC+04:00) Тбилиси", 240),
            new("Asia/Yerevan", "Ереванское время", "Yerevan Time", "(UTC+04:00) Ереван", 240),
            new("Europe/Chisinau", "Молдавское время", "Moldova Time", "(UTC+02:00) Кишинёв", 120),
            new("Asia/Bishkek", "Бишкекское время", "Bishkek Time", "(UTC+06:00) Бишкек", 360),
            new("Europe/Kyiv", "Киевское время", "Kyiv Time", "(UTC+02:00) Киев", 120),
            new(
                "Europe/Berlin",
                "Центральноевропейское",
                "Central European Time",
                "(UTC+01:00) Берлин, Варшава, Прага",
                60
            ),
            new(
                "Europe/London",
                "Западноевропейское",
                "Western European Time",
                "(UTC+00:00) Лондон, Дублин",
                0
            ),
            new(
                "Asia/Dubai",
                "Дубайское время",
                "Gulf Standard Time",
                "(UTC+04:00) Абу-Даби, Дубай",
                240
            ),
            new(
                "Asia/Shanghai",
                "Китайское стандартное",
                "China Standard Time",
                "(UTC+08:00) Пекин, Шанхай",
                480
            ),
            new(
                "Asia/Tokyo",
                "Японское стандартное",
                "Japan Standard Time",
                "(UTC+09:00) Токио, Осака",
                540
            ),
            new(
                "America/New_York",
                "Восточное время (США)",
                "Eastern Time (US & Canada)",
                "(UTC-05:00) Нью-Йорк, Вашингтон",
                -300
            ),
            new(
                "America/Chicago",
                "Центральное время (США)",
                "Central Time (US & Canada)",
                "(UTC-06:00) Чикаго, Даллас",
                -360
            ),
            new(
                "America/Denver",
                "Горное время (США)",
                "Mountain Time (US & Canada)",
                "(UTC-07:00) Денвер, Феникс",
                -420
            ),
            new(
                "America/Los_Angeles",
                "Тихоокеанское время",
                "Pacific Time (US & Canada)",
                "(UTC-08:00) Лос-Анджелес, Сиэтл",
                -480
            ),
            new(
                "UTC",
                "UTC",
                "Coordinated Universal Time",
                "(UTC+00:00) Всемирное координированное время",
                0
            ),
        };

        await context.Set<Timezone>().AddRangeAsync(timezones);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} timezones.", timezones.Length);
    }

    private async Task SeedLanguagesAsync(CatalogDbContext context)
    {
        if (await context.Set<Language>().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding languages...");

        var languages = new Language[]
        {
            // Активные — используются в платформе
            new("ru", "Русский", "Russian", "Русский"),
            new("en", "Английский", "English", "English"),
            new("be", "Белорусский", "Belarusian", "Беларуская"),
            // Неактивные — резерв для будущей локализации
            new("de", "Немецкий", "German", "Deutsch"),
            new("fr", "Французский", "French", "Français"),
            new("zh", "Китайский", "Chinese", "中文"),
            new("ar", "Арабский", "Arabic", "العربية"),
            new("tr", "Турецкий", "Turkish", "Türkçe"),
            new("uk", "Украинский", "Ukrainian", "Українська"),
            new("kk", "Казахский", "Kazakh", "Қазақша"),
            new("uz", "Узбекский", "Uzbek", "O'zbek"),
            new("pl", "Польский", "Polish", "Polski"),
        };

        await context.Set<Language>().AddRangeAsync(languages);
        await context.SaveChangesAsync();

        // Деактивируем языки, не поддерживаемые платформой на текущем этапе
        var activeCodes = new HashSet<string>(["ru", "en", "be"], StringComparer.OrdinalIgnoreCase);
        foreach (var lang in languages.Where(l => !activeCodes.Contains(l.Code)))
        {
            lang.Deactivate();
        }

        await context.SaveChangesAsync();

        logger.LogInformation(
            "Seeded {Count} languages ({ActiveCount} active).",
            languages.Length,
            activeCodes.Count
        );
    }

    private async Task SeedRegionsAsync(CatalogDbContext context)
    {
        if (await context.Set<Region>().AnyAsync())
        {
            return;
        }

        logger.LogInformation("Seeding regions...");

        var regions = new Region[]
        {
            new("CIS", "СНГ", "CIS"),
            new("EU", "Европа", "Europe"),
            new("MENA", "Ближний Восток и Северная Африка", "Middle East & North Africa"),
            new("APAC", "Азиатско-Тихоокеанский регион", "Asia-Pacific"),
            new("NA", "Северная Америка", "North America"),
            new("LATAM", "Латинская Америка", "Latin America"),
            new("SSA", "Африка южнее Сахары", "Sub-Saharan Africa"),
        };

        await context.Set<Region>().AddRangeAsync(regions);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} regions.", regions.Length);
    }
}
