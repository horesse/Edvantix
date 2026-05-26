"use client";

import type React from "react";

import { ShieldAlert } from "lucide-react";

import { AdminGuard as SharedAdminGuard } from "@workspace/auth/admin-guard";
import { useLogout } from "@workspace/auth/use-logout";

/** Shown when the authenticated user lacks the `admin` realm role. */
function AccessDenied() {
  const { logout } = useLogout();

  return (
    <div className="bg-background flex min-h-screen items-center justify-center p-6">
      <div className="flex max-w-sm flex-col items-center gap-4 text-center">
        <div className="bg-destructive/10 flex size-16 items-center justify-center rounded-full">
          <ShieldAlert className="text-destructive size-8" />
        </div>
        <div>
          <h1 className="text-foreground text-xl font-bold">Доступ запрещён</h1>
          <p className="text-muted-foreground mt-1 text-sm">
            У вас нет прав администратора для доступа к этой панели.
          </p>
        </div>
        <button
          type="button"
          onClick={logout}
          className="bg-destructive text-destructive-foreground hover:bg-destructive/90 rounded-lg px-4 py-2 text-sm font-medium"
        >
          Выйти
        </button>
      </div>
    </div>
  );
}

/**
 * Thin app wrapper around the shared AdminGuard.
 * Guards routes that require the `admin` realm role in Keycloak.
 */
export function AdminGuard({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <SharedAdminGuard fallback={<AccessDenied />}>{children}</SharedAdminGuard>
  );
}
