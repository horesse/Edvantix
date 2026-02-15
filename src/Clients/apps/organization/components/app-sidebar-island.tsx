"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import {
  Building,
  ChevronLeft,
  ChevronRight,
  Contact,
  Home,
  Settings,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { Island } from "@workspace/ui/components/island";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import {
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@workspace/ui/components/tooltip";
import { cn } from "@workspace/ui/lib/utils";

import { useOrganization } from "./organization-provider";
import { OrganizationSelector } from "./organization-selector";
import { useSidebarContext } from "./sidebar-context";

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

export function AppSidebarIsland() {
  const pathname = usePathname();
  const { canManage } = useOrganization();
  const { isCollapsed, toggle } = useSidebarContext();

  return (
    <Island
      variant="default"
      padding="sm"
      className={cn(
        "sticky top-0 hidden h-full flex-col transition-all duration-200 lg:flex",
        isCollapsed ? "w-16" : "w-64",
      )}
    >
      <div className="flex items-center justify-between">
        {!isCollapsed && (
          <Link href="/" className="flex items-center gap-2">
            <div className="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
              <Building className="size-4" />
            </div>
            <span className="text-lg font-bold">Edvantix</span>
          </Link>
        )}
        <Button
          variant="ghost"
          size="icon"
          onClick={toggle}
          className={cn("size-8", isCollapsed && "mx-auto")}
        >
          {isCollapsed ? (
            <ChevronRight className="size-4" />
          ) : (
            <ChevronLeft className="size-4" />
          )}
        </Button>
      </div>

      <Separator className="my-3" />

      <ScrollArea className="flex-1">
        <nav className="flex flex-col gap-1">
          {navItems.map((item) => {
            const isActive = item.exact
              ? pathname === item.url
              : pathname.startsWith(item.url) && item.url !== "/";

            const link = (
              <Link
                href={item.url}
                className={cn(
                  "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                  isActive
                    ? "bg-primary text-primary-foreground"
                    : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                  isCollapsed && "justify-center",
                )}
              >
                <item.icon className="size-4 shrink-0" />
                {!isCollapsed && <span>{item.title}</span>}
              </Link>
            );

            if (isCollapsed) {
              return (
                <TooltipProvider key={item.title}>
                  <Tooltip>
                    <TooltipTrigger asChild>{link}</TooltipTrigger>
                    <TooltipContent side="right">
                      <p>{item.title}</p>
                    </TooltipContent>
                  </Tooltip>
                </TooltipProvider>
              );
            }

            return <React.Fragment key={item.title}>{link}</React.Fragment>;
          })}

          {canManage && (
            <>
              <Separator className="my-2" />
              {managementItems.map((item) => {
                const isActive = pathname.startsWith(item.url);

                const link = (
                  <Link
                    href={item.url}
                    className={cn(
                      "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                      isActive
                        ? "bg-primary text-primary-foreground"
                        : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                      isCollapsed && "justify-center",
                    )}
                  >
                    <item.icon className="size-4 shrink-0" />
                    {!isCollapsed && <span>{item.title}</span>}
                  </Link>
                );

                if (isCollapsed) {
                  return (
                    <TooltipProvider key={item.title}>
                      <Tooltip>
                        <TooltipTrigger asChild>{link}</TooltipTrigger>
                        <TooltipContent side="right">
                          <p>{item.title}</p>
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  );
                }

                return <React.Fragment key={item.title}>{link}</React.Fragment>;
              })}
            </>
          )}
        </nav>
      </ScrollArea>

      <Separator className="my-3" />

      <div className="flex flex-col gap-1">
        {!isCollapsed && <OrganizationSelector />}
        {isCollapsed && (
          <Link
            href="/settings"
            className={cn(
              "flex items-center justify-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
              pathname.startsWith("/settings")
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
            )}
          >
            <Settings className="size-4" />
          </Link>
        )}
      </div>
    </Island>
  );
}
