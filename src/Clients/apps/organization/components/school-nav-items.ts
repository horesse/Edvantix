import type { LucideIcon } from "lucide-react";
import {
  BarChart2,
  BookOpen,
  CalendarDays,
  Contact,
  LayoutDashboard,
  MessageSquare,
  Settings,
  UserPlus,
  Users,
  UsersRound,
  Wallet,
} from "lucide-react";

import { OrganizationRole } from "@workspace/types/company";

export interface OrgNavItem {
  id: string;
  title: string;
  url: string;
  icon: LucideIcon;
  /** Use exact pathname match instead of startsWith. */
  exact?: boolean;
  /**
   * Minimum OrganizationRole required to see this item.
   * Role values: Owner=1, Manager=2, Teacher=3, Student=4.
   * Lower number = more privileged. Show item when userRole <= minRole.
   * Undefined = visible to everyone.
   */
  minRole?: OrganizationRole;
}

/**
 * Combined organisation + school management navigation.
 *
 * Since an "organisation" in Edvantix IS a school, all management items
 * live in one ordered list, filtered by the user's role.
 *
 * Ordering rationale: most-used items first, admin-only items last.
 */
const orgNavItems: OrgNavItem[] = [
  {
    id: "dashboard",
    title: "Dashboard",
    url: "/",
    icon: LayoutDashboard,
    exact: true,
  },
  {
    id: "courses",
    title: "Курсы",
    url: "/school/courses",
    icon: BookOpen,
  },
  {
    id: "schedule",
    title: "Расписание",
    url: "/school/schedule",
    icon: CalendarDays,
  },
  {
    // Единый список всех участников школы (ученики + преподаватели).
    // Фильтрация по типу участника происходит на самой странице.
    id: "members",
    title: "Участники",
    url: "/organization/members",
    icon: Users,
    minRole: OrganizationRole.Teacher,
  },
  {
    id: "groups",
    title: "Группы",
    url: "/organization/groups",
    icon: UsersRound,
    minRole: OrganizationRole.Teacher,
  },
  {
    id: "analytics",
    title: "Аналитика",
    url: "/school/analytics",
    icon: BarChart2,
    minRole: OrganizationRole.Teacher,
  },
  {
    id: "invitations",
    title: "Приглашения",
    url: "/organization/invitations",
    icon: UserPlus,
    minRole: OrganizationRole.Manager,
  },
  {
    id: "contacts",
    title: "Контакты",
    url: "/organization/contacts",
    icon: Contact,
    minRole: OrganizationRole.Manager,
  },
  {
    id: "finance",
    title: "Финансы",
    url: "/school/finance",
    icon: Wallet,
    minRole: OrganizationRole.Manager,
  },
  {
    id: "crm",
    title: "CRM",
    url: "/school/crm",
    icon: MessageSquare,
    minRole: OrganizationRole.Manager,
  },
  {
    id: "settings",
    title: "Настройки",
    url: "/organization/settings",
    icon: Settings,
    minRole: OrganizationRole.Manager,
  },
];

/**
 * Returns the nav items visible to the given OrganizationRole.
 * Owner (1) and Manager (2) see everything; Student (4) sees the fewest items.
 */
export function getOrgNavItems(role: OrganizationRole): OrgNavItem[] {
  return orgNavItems.filter(
    (item) => item.minRole === undefined || role <= item.minRole,
  );
}
