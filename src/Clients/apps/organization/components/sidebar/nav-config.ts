import {
  BookOpen,
  GraduationCap,
  LayoutDashboard,
  Settings,
  UserCheck,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

export interface NavItem {
  id: string;
  title: string;
  url: string;
  icon: LucideIcon;
  /** Use exact pathname match instead of startsWith. */
  exact?: boolean;
  /** When true, item is hidden unless user has an organisation selected. */
  requiresOrg?: boolean;
}

export interface NavSection {
  id: string;
  label: string;
  items: NavItem[];
}

const allNavSections: NavSection[] = [
  {
    id: "overview",
    label: "Обзор",
    items: [
      {
        id: "dashboard",
        title: "Дашборд",
        url: "/",
        icon: LayoutDashboard,
        exact: true,
      },
    ],
  },
  {
    id: "people",
    label: "Люди",
    items: [
      {
        id: "students",
        title: "Студенты",
        url: "/organization/members/students",
        icon: GraduationCap,
        requiresOrg: true,
      },
      {
        id: "teachers",
        title: "Работники",
        url: "/organization/members/teachers",
        icon: UserCheck,
        requiresOrg: true,
      },
    ],
  },
  {
    id: "study",
    label: "Учёба",
    items: [
      {
        id: "courses",
        title: "Курсы",
        url: "/school/courses",
        icon: BookOpen,
      },
    ],
  },
  {
    id: "system",
    label: "Система",
    items: [
      {
        id: "settings",
        title: "Настройки",
        url: "/organization/settings",
        icon: Settings,
        requiresOrg: true,
      },
    ],
  },
];

/**
 * Returns navigation sections filtered by whether the user has an organisation.
 * Sections with no visible items are removed entirely.
 */
export function getNavSections(hasOrg: boolean): NavSection[] {
  return allNavSections
    .map((section) => ({
      ...section,
      items: section.items.filter((item) => !item.requiresOrg || hasOrg),
    }))
    .filter((section) => section.items.length > 0);
}
