import {
  LayoutDashboard,
  Settings,
  Users,
} from "lucide-react";
import type { LucideIcon } from "lucide-react";

export interface NavItem {
  id: string;
  title: string;
  url: string;
  icon: LucideIcon;
  exact?: boolean;
}

export interface NavSection {
  id: string;
  label: string;
  items: NavItem[];
}

export const navSections: NavSection[] = [
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
    id: "users",
    label: "Пользователи",
    items: [
      {
        id: "profiles",
        title: "Профили",
        url: "/profiles",
        icon: Users,
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
        url: "/settings",
        icon: Settings,
      },
    ],
  },
];
