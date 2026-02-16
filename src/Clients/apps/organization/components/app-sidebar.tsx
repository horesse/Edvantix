"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import {
  Building,
  Contact,
  Home,
  Settings,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-react";

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarGroupLabel,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from "@workspace/ui/components/sidebar";

import { useOrganization } from "./organization-provider";
import { OrganizationSelector } from "./organization-selector";

const navItems = [
  {
    title: "Главная",
    url: "/",
    icon: Home,
    exact: true,
  },
  {
    title: "Участники",
    url: "/members",
    icon: Users,
    exact: false,
  },
  {
    title: "Приглашения",
    url: "/invitations",
    icon: UserPlus,
    exact: false,
  },
  {
    title: "Группы",
    url: "/groups",
    icon: UsersRound,
    exact: false,
  },
  {
    title: "Контакты",
    url: "/contacts",
    icon: Contact,
    exact: false,
  },
];

const managementItems = [
  {
    title: "Настройки орг.",
    url: "/org-settings",
    icon: Building,
  },
];

const settingsItems = [
  {
    title: "Настройки",
    url: "/settings",
    icon: Settings,
  },
];

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const pathname = usePathname();
  const { canManage } = useOrganization();

  return (
    <Sidebar collapsible="icon" {...props}>
      <SidebarHeader>
        <div className="flex flex-col gap-4">
          <Link href="/" className="flex items-center gap-2 px-2">
            <div className="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
              <Building className="size-5" />
            </div>
            <span className="text-lg font-bold group-data-[collapsible=icon]:hidden">
              Edvantix
            </span>
          </Link>
          <SidebarMenu>
            <SidebarMenuItem>
              <OrganizationSelector />
            </SidebarMenuItem>
          </SidebarMenu>
        </div>
      </SidebarHeader>
      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupLabel>Навигация</SidebarGroupLabel>
          <SidebarGroupContent>
            <SidebarMenu>
              {navItems.map((item) => (
                <SidebarMenuItem key={item.title}>
                  <SidebarMenuButton
                    asChild
                    isActive={
                      item.exact
                        ? pathname === item.url
                        : pathname.startsWith(item.url)
                    }
                  >
                    <Link href={item.url}>
                      <item.icon />
                      <span>{item.title}</span>
                    </Link>
                  </SidebarMenuButton>
                </SidebarMenuItem>
              ))}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
        {canManage && (
          <SidebarGroup>
            <SidebarGroupLabel>Управление</SidebarGroupLabel>
            <SidebarGroupContent>
              <SidebarMenu>
                {managementItems.map((item) => (
                  <SidebarMenuItem key={item.title}>
                    <SidebarMenuButton
                      asChild
                      isActive={pathname.startsWith(item.url)}
                    >
                      <Link href={item.url}>
                        <item.icon />
                        <span>{item.title}</span>
                      </Link>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                ))}
              </SidebarMenu>
            </SidebarGroupContent>
          </SidebarGroup>
        )}
      </SidebarContent>
      <SidebarFooter>
        <SidebarMenu>
          {settingsItems.map((item) => (
            <SidebarMenuItem key={item.title}>
              <SidebarMenuButton
                asChild
                isActive={pathname.startsWith(item.url)}
              >
                <Link href={item.url}>
                  <item.icon />
                  <span>{item.title}</span>
                </Link>
              </SidebarMenuButton>
            </SidebarMenuItem>
          ))}
        </SidebarMenu>
      </SidebarFooter>
    </Sidebar>
  );
}
