import type { LucideIcon } from "lucide-react";
import {
  Building,
  Contact,
  Home,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-react";

export interface OrganizationNavItem {
  id: string;
  title: string;
  url: string;
  icon: LucideIcon;
  exact: boolean;
}

/**
 * Общие элементы навигации для раздела "Организация"
 * Используются в:
 * - Desktop: VerticalNavIsland (иконки с тултипами)
 * - Mobile: MobileSidebar (выпадающий список "Организация")
 */
export const organizationNavItems: OrganizationNavItem[] = [
  {
    id: "home",
    title: "Главная",
    url: "/organization",
    icon: Home,
    exact: true,
  },
  {
    id: "members",
    title: "Участники",
    url: "/organization/members",
    icon: Users,
    exact: false,
  },
  {
    id: "invitations",
    title: "Приглашения",
    url: "/organization/invitations",
    icon: UserPlus,
    exact: false,
  },
  {
    id: "groups",
    title: "Группы",
    url: "/organization/groups",
    icon: UsersRound,
    exact: false,
  },
  {
    id: "contacts",
    title: "Контакты",
    url: "/organization/contacts",
    icon: Contact,
    exact: false,
  },
  {
    id: "org-settings",
    title: "Настройки организации",
    url: "/organization/settings",
    icon: Building,
    exact: false,
  },
];
