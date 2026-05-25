import {
  Bell,
  BookOpen,
  Building2,
  CalendarDays,
  CreditCard,
  FileText,
  Layers,
  Megaphone,
  Shield,
  Sparkles,
  UserCheck,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

/** Маппинг строкового кода иконки → компонент Lucide. */
export const DIRECTORY_ICONS: Record<string, LucideIcon> = {
  Layers,
  BookOpen,
  CalendarDays,
  UserCheck,
  Building2,
  Megaphone,
  CreditCard,
  Sparkles,
  FileText,
  Shield,
  Bell,
};

/** Маппинг кода справочника → маршрут приложения.
 *  Только справочники с готовыми страницами получают href. */
export const DIRECTORY_ROUTES: Record<string, string> = {
  levels: "/organization/levels",
  subjects: "/organization/subjects",
  "lesson-types": "/organization/lesson-types",
  "payment-methods": "/organization/payment-methods",
};

export type PlatformItem = {
  readonly id: string;
  readonly name: string;
  readonly icon: LucideIcon;
  readonly description: string;
  readonly meta: string;
  readonly tone: PlatformTone;
};

export type PlatformTone =
  | "indigo"
  | "violet"
  | "amber"
  | "emerald"
  | "rose"
  | "slate";

/** Цветовые пары bg/fg для карточек платформы по тональности. */
export const PLATFORM_TONE_COLORS: Record<
  PlatformTone,
  { bg: string; fg: string }
> = {
  indigo: { bg: "rgba(79,70,229,0.10)", fg: "#4338ca" },
  violet: { bg: "rgba(139,92,246,0.10)", fg: "#6d28d9" },
  amber: { bg: "rgba(245,158,11,0.12)", fg: "#92400e" },
  emerald: { bg: "rgba(16,185,129,0.12)", fg: "#047857" },
  rose: { bg: "rgba(244,63,94,0.10)", fg: "#be123c" },
  slate: { bg: "rgba(100,116,139,0.10)", fg: "#475569" },
};

/** Статичные заглушки «скоро» для секции Платформа. */
export const PLATFORM_ITEMS: readonly PlatformItem[] = [
  {
    id: "notifications",
    name: "Уведомления",
    icon: Bell,
    description: "Шаблоны email и SMS, расписание автосообщений.",
    meta: "12 шаблонов",
    tone: "indigo",
  },
  {
    id: "integrations",
    name: "Интеграции",
    icon: Sparkles,
    description: "Платёжки, мессенджеры, телефония, аналитика.",
    meta: "3 из 12 подключено",
    tone: "violet",
  },
  {
    id: "branding",
    name: "Брендинг",
    icon: Shield,
    description: "Логотип, цвет, поддомен и фирменные письма.",
    meta: "настроено",
    tone: "amber",
  },
  {
    id: "security",
    name: "Безопасность",
    icon: Shield,
    description:
      "Двухфакторная аутентификация, политика паролей, активные сессии.",
    meta: "2FA включён",
    tone: "emerald",
  },
  {
    id: "billing",
    name: "Биллинг и тариф",
    icon: CreditCard,
    description: "Текущий тариф, история платежей, лимиты.",
    meta: "Pro · до 12 июня",
    tone: "rose",
  },
  {
    id: "audit",
    name: "Журнал действий",
    icon: FileText,
    description: "История изменений и действий пользователей.",
    meta: "247 за неделю",
    tone: "slate",
  },
] as const;

/** Маршрут страницы редактирования организации. */
export const ORG_EDIT_ROUTE = "/org-settings";
