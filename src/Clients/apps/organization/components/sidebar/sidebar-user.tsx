"use client";

import Link from "next/link";

import { ChevronsUpDown, LogOut, Settings } from "lucide-react";

import useOwnProfile from "@workspace/api-hooks/profiles/useOwnProfile";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";
import { Skeleton } from "@workspace/ui/components/skeleton";
import { getInitials } from "@workspace/utils/format";

import { useOrganization } from "@/components/organization/provider";
import { useLogout } from "@/hooks/useLogout";

const ROLE_LABELS: Record<string, string> = {
  Owner: "Владелец",
  Manager: "Менеджер",
  Teacher: "Учитель",
  Student: "Ученик",
};

/**
 * User profile widget at the bottom of the sidebar.
 * Shows avatar initials, full name, and organization role.
 * Click opens a dropdown with profile settings and logout.
 */
export function SidebarUser() {
  const { data: profile, isLoading } = useOwnProfile();
  const { userRole } = useOrganization();
  const { logout } = useLogout();

  if (isLoading) {
    return (
      <div className="flex items-center gap-2.5 px-3 py-2">
        <Skeleton className="size-8 shrink-0 rounded-full" />
        <div className="flex-1 space-y-1.5">
          <Skeleton className="h-3.5 w-24 rounded" />
          <Skeleton className="h-3 w-16 rounded" />
        </div>
      </div>
    );
  }

  if (!profile) return null;

  const initials = getInitials(profile.name);
  const roleName = userRole
    ? (ROLE_LABELS[String(userRole)] ?? String(userRole))
    : null;

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button
          type="button"
          className="hover:bg-sidebar-accent flex w-full items-center gap-2.5 rounded-xl px-3 py-2 text-left transition-colors"
        >
          <div className="from-brand-400 to-brand-600 flex size-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br">
            <span className="text-primary-foreground text-xs font-semibold">
              {initials}
            </span>
          </div>
          <div className="min-w-0 flex-1">
            <p className="text-foreground truncate text-sm font-medium">
              {profile.name}
            </p>
            {roleName && (
              <p className="text-sidebar-foreground/60 truncate text-xs">
                {roleName}
              </p>
            )}
          </div>
          <ChevronsUpDown className="text-sidebar-foreground/40 size-4 shrink-0" />
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent side="top" align="start" className="w-56">
        <DropdownMenuLabel className="font-normal">
          <div className="flex flex-col space-y-1">
            <p className="text-sm leading-none font-medium">{profile.name}</p>
            <p className="text-muted-foreground text-xs leading-none">
              {profile.userName}
            </p>
          </div>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem asChild>
          <Link href="/settings/profile">
            <Settings />
            <span>Настройки профиля</span>
          </Link>
        </DropdownMenuItem>
        <DropdownMenuSeparator />
        <DropdownMenuItem variant="destructive" onClick={logout}>
          <LogOut />
          <span>Выйти</span>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
