import {
  BarChart2,
  BookOpen,
  Briefcase,
  Building2,
  CalendarDays,
  FileText,
  GraduationCap,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

/** Иконка и описание для группы прав по коду фичи. */
export const FEATURE_META: Record<
  string,
  { icon: LucideIcon; description: string }
> = {
  students: {
    icon: GraduationCap,
    description: "Профили учеников, зачисления, родители",
  },
  courses: {
    icon: BookOpen,
    description: "Программы, уроки, материалы",
  },
  schedule: {
    icon: CalendarDays,
    description: "Занятия, группы, преподаватели",
  },
  attendance: {
    icon: BarChart2,
    description: "Журналы, отметки, пропуски",
  },
  finance: {
    icon: Briefcase,
    description: "Платежи, договоры, задолженности",
  },
  reports: {
    icon: FileText,
    description: "Аналитика и выгрузки",
  },
  org: {
    icon: Building2,
    description: "Реквизиты, участники, роли",
  },
};

/** Палитры цветов для аватарок ролей — выбираются детерминированно по имени. */
const ROLE_PALETTES: ReadonlyArray<{ bg: string; fg: string }> = [
  { bg: "rgba(139,92,246,0.12)", fg: "#6d28d9" }, // violet
  { bg: "rgba(79,70,229,0.12)", fg: "#4338ca" }, // indigo
  { bg: "rgba(14,165,233,0.12)", fg: "#0369a1" }, // blue
  { bg: "rgba(20,184,166,0.12)", fg: "#0f766e" }, // teal
  { bg: "rgba(245,158,11,0.14)", fg: "#92400e" }, // amber
  { bg: "#f1f5f9", fg: "#475569" }, // slate
  { bg: "rgba(236,72,153,0.12)", fg: "#9d174d" }, // pink
  { bg: "rgba(16,185,129,0.12)", fg: "#047857" }, // emerald
];

function hashString(s: string): number {
  let hash = 0;
  for (let i = 0; i < s.length; i++) {
    hash = (hash * 31 + (s.codePointAt(i) ?? 0)) >>> 0;
  }

  return hash;
}

/** Возвращает детерминированную цветовую пару bg/fg по имени роли. */
export function getRoleAvatarColors(name: string): { bg: string; fg: string } {
  return (
    ROLE_PALETTES[hashString(name) % ROLE_PALETTES.length] ?? {
      bg: "#f1f5f9",
      fg: "#475569",
    }
  );
}

/** Склонение слова «участник» по числу. */
export function declMembers(n: number): string {
  const a = Math.abs(n);
  const m10 = a % 10;
  const m100 = a % 100;
  if (m10 === 1 && m100 !== 11) return "участник";
  if (m10 >= 2 && m10 <= 4 && (m100 < 10 || m100 >= 20)) return "участника";

  return "участников";
}
