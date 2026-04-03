"use client";

import type React from "react";
import { useEffect, useState } from "react";

import { ShieldAlert } from "lucide-react";

import { signOut } from "@/lib/auth-client";

/** Decodes the JWT payload from localStorage and returns realm_access.roles. */
function getRealmRoles(): string[] {
  if (typeof window === "undefined") return [];
  const token = window.localStorage.getItem("access_token");
  if (!token) return [];
  try {
    const [, payload] = token.split(".");
    if (!payload) return [];
    const padded = payload + "==".slice((2 - (payload.length % 4)) % 4);
    const decoded = JSON.parse(atob(padded));
    return (decoded?.realm_access?.roles as string[]) ?? [];
  } catch {
    return [];
  }
}

/**
 * Guards routes that require the `admin` realm role.
 * Reads realm roles from the decoded JWT access token stored in localStorage.
 */
export function AdminGuard({ children }: Readonly<{ children: React.ReactNode }>) {
  const [isAdmin, setIsAdmin] = useState<boolean | null>(null);

  useEffect(() => {
    const roles = getRealmRoles();
    setIsAdmin(roles.includes("admin"));
  }, []);

  // Still checking token
  if (isAdmin === null) {
    return null;
  }

  if (!isAdmin) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-background p-6">
        <div className="flex max-w-sm flex-col items-center gap-4 text-center">
          <div className="flex size-16 items-center justify-center rounded-full bg-destructive/10">
            <ShieldAlert className="size-8 text-destructive" />
          </div>
          <div>
            <h1 className="text-xl font-bold text-foreground">Доступ запрещён</h1>
            <p className="mt-1 text-sm text-muted-foreground">
              У вас нет прав администратора для доступа к этой панели.
            </p>
          </div>
          <button
            onClick={() => signOut()}
            className="rounded-lg bg-destructive px-4 py-2 text-sm font-medium text-destructive-foreground hover:bg-destructive/90"
          >
            Выйти
          </button>
        </div>
      </div>
    );
  }

  return <>{children}</>;
}
