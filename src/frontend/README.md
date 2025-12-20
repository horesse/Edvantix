# Edvantix Frontend

Микрофронтенд архитектура для Edvantix - системы управления онлайн школой.

## Приложения

### Apps

- **`auth`** - Приложение авторизации с красивым UI (порт 3001)
  - Вход и регистрация
  - Восстановление пароля
  - Mock реализация без бэкенд логики

- **`main`** - Основное приложение (панель управления) (порт 3002)
  - Dashboard с метриками
  - Управление студентами, курсами, преподавателями
  - Пустая заготовка для дальнейшей разработки

- **`landing`** - Landing page с SSR (порт 3000)
  - Полностью серверный рендеринг (SSR)
  - SEO оптимизация
  - Презентация возможностей платформы

### Packages

- `@repo/ui`: Общие React компоненты
- `@repo/eslint-config`: Конфигурация ESLint
- `@repo/typescript-config`: Конфигурация TypeScript

## Технологический стек

- **Framework**: Next.js 15 (App Router)
- **React**: 19
- **TypeScript**: 5.9
- **Styling**: Tailwind CSS 3.4
- **UI Components**: shadcn/ui (Radix UI + Tailwind)
- **Icons**: Lucide React
- **Monorepo**: Turborepo
- **Package Manager**: Bun

## Установка

```bash
bun install
```

## Запуск в разработке

### Все приложения одновременно

```bash
bun dev
```

Приложения будут доступны по адресам:
- Landing: http://localhost:3000
- Auth: http://localhost:3001
- Main: http://localhost:3002

### Отдельные приложения

```bash
# Landing
turbo dev --filter=@edvantix/landing

# Auth
turbo dev --filter=@edvantix/auth

# Main
turbo dev --filter=@edvantix/main
```

## Сборка

### Все приложения

```bash
bun build
```

### Отдельные приложения

```bash
# Landing
turbo build --filter=@edvantix/landing

# Auth
turbo build --filter=@edvantix/auth

# Main
turbo build --filter=@edvantix/main
```

## Проверка типов

```bash
bun check-types
```

## Линтинг

```bash
bun lint
```

## Форматирование

```bash
bun format
```

## Архитектура

Проект построен на микрофронтенд архитектуре с использованием Turborepo:

```
frontend/
├── apps/
│   ├── auth/          # Приложение авторизации
│   ├── main/          # Основное приложение
│   └── landing/       # Landing page
├── packages/
│   ├── ui/            # Общие UI компоненты
│   ├── eslint-config/ # ESLint конфигурация
│   └── typescript-config/ # TypeScript конфигурация
```

### Особенности реализации

#### shadcn/ui компоненты

Установлены базовые компоненты:
- Button
- Input
- Card (CardHeader, CardTitle, CardDescription, CardContent, CardFooter)
- Label

Каждое приложение имеет свою копию компонентов для независимого развития.

#### SSR в Landing

Landing использует серверный рендеринг по умолчанию (Next.js App Router).
Все компоненты страницы - серверные, что обеспечивает:
- Быструю загрузку контента
- SEO оптимизацию
- Лучшую производительность

#### Микрофронтенд подход

Каждое приложение:
- Независимо деплоится
- Имеет свой порт
- Может использовать свою версию зависимостей
- Развивается отдельной командой

## Следующие шаги

1. Добавить API интеграцию
2. Настроить модуль-федерацию для переиспользования компонентов
3. Добавить состояние (Zustand/Redux)
4. Настроить аутентификацию (NextAuth.js)
5. Добавить i18n (next-intl)
6. Настроить CI/CD

## Полезные ссылки

- [Next.js Documentation](https://nextjs.org/docs)
- [shadcn/ui](https://ui.shadcn.com)
- [Tailwind CSS](https://tailwindcss.com/docs)
- [Turborepo](https://turborepo.com/docs)
