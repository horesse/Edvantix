# Edvantix.OrganizationManagement

Микросервис управления организациями, контактами, членами и использованием лимитов.

## Описание

Этот микросервис предоставляет функциональность для:
- **Организации** - управление данными организаций (только чтение через API)
- **Контакты** - полный CRUD для контактов организаций
- **Члены (Members)** - полный CRUD для членов организаций с поддержкой soft delete
- **Использование (Usage)** - мониторинг использования лимитов (только чтение через API, запись через gRPC)
- **Подписки** - связь организаций с подписками системы

## Архитектура

### Domain Layer
- **Organization Aggregate** - основная сущность организации
- **Contact Aggregate** - контакты организаций (Email, Phone, Uri, Other)
- **Member Aggregate** - члены организаций с soft delete
- **Usage Aggregate** - использование лимитов
- **Subscription Aggregate** - подписки организаций

### Infrastructure Layer
- PostgreSQL база данных через Entity Framework Core
- Репозитории с использованием `ICrudRepository` и `SoftDeleteRepository`
- Entity Configurations для всех агрегатов
- Migrations для версионирования схемы БД

### Application Layer (Features)

#### Contact Feature
- **CRUD API**: Полное управление контактами
- **Endpoints**: Create, Read, Update, Delete
- **Specification**: Фильтрация по OrganizationId, Type

#### Member Feature
- **CRUD API**: Полное управление членами
- **Endpoints**: Create, Read, Update, Delete (soft delete)
- **Specification**: Фильтрация по OrganizationId, PersonId
- **Особенности**: Поддержка soft delete и query filter

#### Organization Feature
- **READ-ONLY API**: Только чтение организаций
- **Endpoints**: GetById, GetByExpression, FetchPagedData
- **Specification**: Поиск по Name, NameLatin
- **Примечание**: Создание и обновление организаций производится вне API

#### Usage Feature
- **READ-ONLY API**: Мониторинг использования лимитов
- **Endpoints**: GetById, GetByExpression, FetchPagedData
- **gRPC Service**: Запись и обновление данных использования
- **Specification**: Фильтрация по OrganizationId, LimitId

### gRPC Services

#### UsageService
- **Method**: `UpdateUsageValue`
- **Purpose**: Обновление значений использования лимитов
- **Proto**: `usage/v1/usage.proto`
- **Features**:
  - Создание новой записи использования если не существует
  - Обновление существующей записи
  - Автоматическое управление транзакциями

## API Endpoints

### Contact Endpoints
- `GET /api/v1/contact/{id}` - Получить контакт по ID
- `POST /api/v1/contact/fetch-paged` - Получить постраничный список
- `POST /api/v1/contact` - Создать новый контакт
- `PUT /api/v1/contact` - Обновить контакт
- `DELETE /api/v1/contact/{id}` - Удалить контакт

### Member Endpoints
- `GET /api/v1/member/{id}` - Получить члена по ID
- `POST /api/v1/member/fetch-paged` - Получить постраничный список
- `POST /api/v1/member` - Создать нового члена
- `PUT /api/v1/member` - Обновить члена
- `DELETE /api/v1/member/{id}` - Удалить члена (soft delete)

### Organization Endpoints
- `GET /api/v1/organization/{id}` - Получить организацию по ID
- `POST /api/v1/organization/fetch-paged` - Получить постраничный список
- `POST /api/v1/organization/by-expression` - Поиск по выражению

### Usage Endpoints
- `GET /api/v1/usage/{id}` - Получить использование по ID
- `POST /api/v1/usage/fetch-paged` - Получить постраничный список
- `POST /api/v1/usage/by-expression` - Поиск по выражению

## gRPC Endpoints

### Usage Service
```protobuf
service UsageGrpcService {
  rpc UpdateUsageValue(UpdateUsageRequest) returns (UsageResponse);
}
```

## Конфигурация

### Aspire Integration
Сервис зарегистрирован в Aspire AppHost:
```csharp
var organizationApi = builder
    .AddProject<Edvantix_OrganizationManagement>(Services.Organization)
    .WithReference(organizationDb)
    .WaitFor(organizationDb)
    .WithKeycloak(keycloak)
    .WithFriendlyUrls();
```

### Database
- **Provider**: PostgreSQL
- **Connection String**: Управляется через Aspire
- **Database Name**: `organizationdb`
- **Migrations**: Автоматически применяются при запуске в Development режиме

### Authentication & Authorization
- **Provider**: Keycloak
- **Default Policy**: Требуется аутентификация + scope `organization_read`
- **Admin Policy**: Требуется роль Admin + scopes `organization_read` и `organization_write`

## Технологии

- **.NET 10.0**
- **ASP.NET Core Web API**
- **Entity Framework Core** с PostgreSQL
- **gRPC** для межсервисной коммуникации
- **MediatR** для CQRS паттерна
- **FluentValidation** для валидации
- **Aspire** для оркестрации
- **OpenAPI/Swagger** для документации API

## Зависимости

### NuGet Packages
- `Grpc.Tools` - gRPC кодогенерация
- `MassTransit.EntityFrameworkCore` - интеграция с EF Core
- `Microsoft.EntityFrameworkCore.Tools` - инструменты EF Core
- `Microsoft.Extensions.Caching.Hybrid` - гибридное кеширование

### Project References
- `Edvantix.ServiceDefaults` - общие настройки сервисов
- `Edvantix.Chassis` - базовые компоненты (Repository, CQRS, Endpoints)
- `Edvantix.Constants` - константы приложения
- `Edvantix.SharedKernel` - общие доменные типы

## Разработка

### Создание миграции
```bash
cd src/Services/Organization/Edvantix.OrganizationManagement
dotnet ef migrations add MigrationName
```

### Применение миграций
Миграции применяются автоматически при запуске через Aspire в Development режиме.

### Запуск
```bash
cd src/Aspire/Edvantix.Aspire
dotnet run
```

## Модели данных

### Organization
- Id (long)
- Name (string) - Название
- NameLatin (string) - Латинское название
- ShortName (string) - Краткое название
- PrintName (string?) - Печатное название
- Description (string?) - Описание
- RegistrationDate (DateTime) - Дата регистрации

### Contact
- Id (long)
- OrganizationId (long)
- Type (ContactType) - Email, Phone, Uri, Other
- Value (string) - Значение контакта
- Description (string?) - Описание

### Member
- Id (Guid)
- OrganizationId (long)
- PersonId (Guid)
- Position (string?) - Должность
- IsDeleted (bool) - Флаг удаления

### Usage
- Id (long)
- OrganizationId (long)
- LimitId (long)
- Value (decimal) - Значение использования

### Subscription
- Id (long)
- SubscriptionId (long) - Ссылка на подписку в System сервисе
- OrganizationId (long)
- DateStart (DateTime) - Дата начала
- DateEnd (DateTime?) - Дата окончания

## Особенности реализации

1. **Soft Delete для Members**: Члены организации не удаляются физически, используется флаг `IsDeleted`
2. **Read-Only API для Organization**: Организации создаются вне API, API предоставляет только чтение
3. **gRPC для Usage**: Обновление использования лимитов происходит через gRPC для производительности
4. **CQRS Pattern**: Разделение команд и запросов через MediatR
5. **Generic Endpoints**: Использование generic endpoints из Chassis для стандартных операций
6. **Specification Pattern**: Использование спецификаций для сложных запросов

## Связи с другими сервисами

- **System Service**: Получение информации о лимитах через gRPC
- **Gateway**: Проксирование HTTP запросов через Yarp
- **Keycloak**: Аутентификация и авторизация
- **PostgreSQL**: Хранение данных

