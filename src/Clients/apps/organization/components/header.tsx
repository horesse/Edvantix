"use client";

import * as React from "react";

import Link from "next/link";

import { LogOut, Search, Settings as SettingsIcon, User } from "lucide-react";

import useOwnProfile from "@workspace/api-hooks/profiles/useOwnProfile";
import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from "@workspace/ui/components/avatar";
import { Button } from "@workspace/ui/components/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
import { Separator } from "@workspace/ui/components/separator";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { getInitials } from "@workspace/utils/format";

import { useLogout } from "@/hooks/useLogout";

import { MobileSidebar } from "./mobile-sidebar";
import { NotificationBell } from "./notification-bell";
import { PageBreadcrumb } from "./page-breadcrumb";
import { ThemeToggle } from "./theme-toggle";

/**
 * Top application bar — slim, flat design with border-b only (no shadow).
 * Contains: mobile nav trigger, breadcrumb, actions, user avatar.
 */
export function Header() {
  const { data: profile, isLoading } = useOwnProfile();
  const { logout } = useLogout();

  return (
    <header className="border-border flex h-12 shrink-0 items-center gap-3 border-b px-4">
      <MobileSidebar />
      <PageBreadcrumb />

      <div className="ml-auto flex items-center gap-1">
        <Button
          variant="ghost"
          size="icon"
          className="size-8 rounded-full"
          aria-label="Поиск"
        >
          <Search className="size-4" />
        </Button>
        <NotificationBell />
        <ThemeToggle />
        <Separator orientation="vertical" className="mx-1 h-4" />
        {isLoading ? (
          <Skeleton className="size-8 rounded-full" />
        ) : (
          profile && (
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="ghost"
                  className="relative size-8 rounded-full"
                >
                  <Avatar className="size-8">
                    <AvatarImage
                      src={profile.avatarUrl}
                      alt={profile.name}
                      itemProp="image"
                    />
                    <AvatarFallback>{getInitials(profile.name)}</AvatarFallback>
                  </Avatar>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end" className="w-56">
                <DropdownMenuLabel className="font-normal">
                  <div className="flex flex-col space-y-1">
                    <p className="text-sm leading-none font-medium">
                      {profile.name}
                    </p>
                    <p className="text-muted-foreground text-xs leading-none">
                      {profile.userName}
                    </p>
                  </div>
                </DropdownMenuLabel>
                <DropdownMenuSeparator />
                <DropdownMenuItem asChild>
                  <Link href="/settings/profile">
                    <User />
                    <span>Профиль</span>
                  </Link>
                </DropdownMenuItem>
                <DropdownMenuItem asChild>
                  <Link href="/settings">
                    <SettingsIcon />
                    <span>Настройки</span>
                  </Link>
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem variant="destructive" onClick={logout}>
                  <LogOut />
                  <span>Выйти</span>
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          )
        )}
      </div>
    </header>
  );
}
