"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import {
  Building,
  Contact,
  Home,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-react";

import { Island } from "@workspace/ui/components/island";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@workspace/ui/components/tooltip";
import { cn } from "@workspace/ui/lib/utils";

const navItems = [
  {
    id: "home",
    label: "Главная",
    url: "/organization",
    icon: Home,
    exact: true,
  },
  {
    id: "members",
    label: "Участники",
    url: "/organization/members",
    icon: Users,
    exact: false,
  },
  {
    id: "invitations",
    label: "Приглашения",
    url: "/organization/invitations",
    icon: UserPlus,
    exact: false,
  },
  {
    id: "groups",
    label: "Группы",
    url: "/organization/groups",
    icon: UsersRound,
    exact: false,
  },
  {
    id: "contacts",
    label: "Контакты",
    url: "/organization/contacts",
    icon: Contact,
    exact: false,
  },
  {
    id: "org-settings",
    label: "Настройки организации",
    url: "/organization/settings",
    icon: Building,
    exact: false,
  },
];

export function VerticalNavIsland() {
  const pathname = usePathname();

  return (
    <Island
      variant="default"
      padding="none"
      rounded="lg"
      className="sticky top-0 hidden h-full flex-col items-center justify-center p-2 lg:flex"
    >
      <div
        className="absolute left-1/2 top-4 -translate-x-1/2 text-xs font-medium text-muted-foreground/50"
        style={{ writingMode: "vertical-rl", textOrientation: "mixed" }}
      >
        Организация
      </div>

      <nav className="flex flex-col items-center justify-center gap-1.5">
        {navItems.map((item) => {
          const isActive = item.exact
            ? pathname === item.url
            : pathname.startsWith(item.url) && item.url !== "/organization";

          return (
            <TooltipProvider key={item.id}>
              <Tooltip>
                <TooltipTrigger asChild>
                  <Link
                    href={item.url}
                    className={cn(
                      "flex size-8 items-center justify-center rounded-md transition-colors",
                      isActive
                        ? "bg-primary text-primary-foreground"
                        : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                    )}
                  >
                    <item.icon className="size-4" />
                    <span className="sr-only">{item.label}</span>
                  </Link>
                </TooltipTrigger>
                <TooltipContent side="right">
                  <div className="flex items-center gap-2">
                    <item.icon className="size-3.5" />
                    <span>{item.label}</span>
                  </div>
                </TooltipContent>
              </Tooltip>
            </TooltipProvider>
          );
        })}
      </nav>
    </Island>
  );
}
