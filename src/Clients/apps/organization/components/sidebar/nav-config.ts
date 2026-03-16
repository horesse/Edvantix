import {
  BarChart2,
  BookOpen,
  CalendarDays,
  ClipboardCheck,
  GraduationCap,
  LayoutDashboard,
  Settings,
  UserCheck,
  Users,
  UsersRound,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

import { OrganizationRole } from "@workspace/types/company";

export interface NavItem {
  id: string;
  title: string;
  url: string;
  icon: LucideIcon;
  /** Use exact pathname match instead of startsWith. */
  exact?: boolean;
  /** Minimum role required (Owner=1 is most privileged, Student=4 is least). */
  minRole?: OrganizationRole;
}

export interface NavSection {
  id: string;
  label: string;
  items: NavItem[];
}

/**
 * Navigation sections for the organization microfrontend.
 * Sections are derived from the edvantix-members.html design reference.
 * Organization-level backend features (billing, CRM, contacts) are
 * intentionally omitted pending backend redesign.
 */
const allNavSections: NavSection[] = [
  {
    id: "overview",
    label: "Обзор",
    items: [
      {
        id: "dashboard",
        title: "Главная",
        url: "/",
        icon: LayoutDashboard,
        exact: true,
      },
      {
        id: "analytics",
        title: "Аналитика",
        url: "/school/analytics",
        icon: BarChart2,
        minRole: OrganizationRole.Teacher,
      },
    ],
  },
  {
    id: "people",
    label: "Люди",
    items: [
      {
        id: "members",
        title: "Участники",
        url: "/organization/members",
        icon: Users,
        minRole: OrganizationRole.Teacher,
      },
      {
        id: "students",
        title: "Ученики",
        url: "/organization/members/students",
        icon: GraduationCap,
        minRole: OrganizationRole.Teacher,
      },
      {
        id: "teachers",
        title: "Учителя",
        url: "/organization/members/teachers",
        icon: UserCheck,
        minRole: OrganizationRole.Teacher,
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
      {
        id: "groups",
        title: "Группы",
        url: "/organization/groups",
        icon: UsersRound,
        minRole: OrganizationRole.Teacher,
      },
      {
        id: "attendance",
        title: "Посещаемость",
        url: "/school/attendance",
        icon: ClipboardCheck,
      },
      {
        id: "schedule",
        title: "Расписание",
        url: "/school/schedule",
        icon: CalendarDays,
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
        minRole: OrganizationRole.Manager,
      },
    ],
  },
];

/**
 * Returns sections filtered by the user's role.
 * Sections with no visible items after filtering are removed entirely.
 */
export function getNavSections(role: OrganizationRole): NavSection[] {
  return allNavSections
    .map((section) => ({
      ...section,
      items: section.items.filter(
        (item) => item.minRole === undefined || role <= item.minRole,
      ),
    }))
    .filter((section) => section.items.length > 0);
}
