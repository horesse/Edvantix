"use client";

import { LogOut, Shield } from "lucide-react";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@workspace/ui/components/dropdown-menu";

import { useUserContext } from "@/hooks/use-user-context";
import { signOut } from "@/lib/auth-client";

export function SidebarUser() {
  const { user } = useUserContext();

  const name = user?.name ?? "Администратор";
  const initials = name
    .split(" ")
    .slice(0, 2)
    .map((w: string) => w[0])
    .join("")
    .toUpperCase();

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <button className="hover:bg-sidebar-accent flex w-full items-center gap-2.5 rounded-lg px-3 py-2 text-sm transition-colors">
          <div className="from-primary/60 to-primary flex size-8 shrink-0 items-center justify-center rounded-full bg-gradient-to-br text-xs font-bold text-white">
            {initials}
          </div>
          <div className="min-w-0 flex-1 text-left">
            <p className="text-sidebar-foreground truncate text-sm leading-none font-medium">
              {name}
            </p>
            <p className="text-sidebar-foreground/50 mt-0.5 flex items-center gap-1 text-[11px]">
              <Shield className="size-3" />
              Администратор
            </p>
          </div>
        </button>
      </DropdownMenuTrigger>
      <DropdownMenuContent side="top" align="start" className="w-52">
        <DropdownMenuLabel className="font-normal">
          <p className="text-sm font-medium">{name}</p>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuItem variant="destructive" onClick={() => signOut()}>
          <LogOut className="size-4" />
          Выйти
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
}
