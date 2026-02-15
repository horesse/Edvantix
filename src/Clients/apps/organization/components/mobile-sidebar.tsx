"use client";

import * as React from "react";

import Link from "next/link";
import { usePathname } from "next/navigation";

import {
  Building,
  Contact,
  Home,
  Menu,
  Settings,
  UserPlus,
  Users,
  UsersRound,
} from "lucide-react";

import { Button } from "@workspace/ui/components/button";
import { ScrollArea } from "@workspace/ui/components/scroll-area";
import { Separator } from "@workspace/ui/components/separator";
import {
  Sheet,
  SheetContent,
  SheetHeader,
  SheetTitle,
  SheetTrigger,
} from "@workspace/ui/components/sheet";
import { cn } from "@workspace/ui/lib/utils";

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

export function MobileSidebar() {
  const pathname = usePathname();
  const { canManage } = useOrganization();
  const [open, setOpen] = React.useState(false);

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger asChild>
        <Button variant="ghost" size="icon" className="lg:hidden">
          <Menu className="size-5" />
          <span className="sr-only">Открыть меню</span>
        </Button>
      </SheetTrigger>
      <SheetContent side="left" className="w-64 p-0">
        <SheetHeader className="p-4">
          <SheetTitle className="flex items-center gap-2">
            <div className="flex size-8 items-center justify-center rounded-lg bg-primary text-primary-foreground">
              <Building className="size-4" />
            </div>
            <span className="text-lg font-bold">Edvantix</span>
          </SheetTitle>
        </SheetHeader>
        <Separator />
        <ScrollArea className="h-[calc(100vh-8rem)] px-4 py-4">
          <nav className="flex flex-col gap-1">
            {navItems.map((item) => {
              const isActive = item.exact
                ? pathname === item.url
                : pathname.startsWith(item.url) && item.url !== "/";

              return (
                <Link
                  key={item.title}
                  href={item.url}
                  onClick={() => setOpen(false)}
                  className={cn(
                    "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                    isActive
                      ? "bg-primary text-primary-foreground"
                      : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                  )}
                >
                  <item.icon className="size-4" />
                  <span>{item.title}</span>
                </Link>
              );
            })}

            {canManage && (
              <>
                <Separator className="my-2" />
                {managementItems.map((item) => {
                  const isActive = pathname.startsWith(item.url);

                  return (
                    <Link
                      key={item.title}
                      href={item.url}
                      onClick={() => setOpen(false)}
                      className={cn(
                        "flex items-center gap-2 rounded-md px-2 py-1.5 text-sm font-medium transition-colors",
                        isActive
                          ? "bg-primary text-primary-foreground"
                          : "text-muted-foreground hover:bg-accent hover:text-accent-foreground",
                      )}
                    >
                      <item.icon className="size-4" />
                      <span>{item.title}</span>
                    </Link>
                  );
                })}
              </>
            )}
          </nav>
        </ScrollArea>
        <div className="absolute bottom-0 left-0 right-0 border-t p-4">
          <OrganizationSelector />
        </div>
      </SheetContent>
    </Sheet>
  );
}
