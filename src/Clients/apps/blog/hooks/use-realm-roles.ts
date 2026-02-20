"use client";

import { useEffect, useState } from "react";

import { getAccessToken, useSession } from "@/lib/auth-client";
import { AUTH } from "@/lib/constants";

const LS_KEY = "access_token";

type KeycloakTokenPayload = {
  exp?: number;
  realm_access?: { roles?: string[] };
  resource_access?: Record<string, { roles?: string[] }>;
};

function decodeJwtPayload(token: string): KeycloakTokenPayload | null {
  try {
    const base64 = token.split(".")[1];
    if (!base64) return null;
    const json = atob(base64.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(json) as KeycloakTokenPayload;
  } catch {
    return null;
  }
}

function isTokenExpired(payload: KeycloakTokenPayload): boolean {
  if (!payload.exp) return false;
  return Date.now() / 1000 > payload.exp;
}

function readToken(): string | null {
  try {
    return localStorage.getItem(LS_KEY);
  } catch {
    return null;
  }
}

function writeToken(token: string): void {
  try {
    localStorage.setItem(LS_KEY, token);
  } catch {
    // ignore (private browsing / quota)
  }
}

function clearToken(): void {
  try {
    localStorage.removeItem(LS_KEY);
  } catch {
    // ignore
  }
}

function getRolesFromToken(token: string): string[] {
  const payload = decodeJwtPayload(token);
  if (!payload || isTokenExpired(payload)) return [];
  return payload.realm_access?.roles ?? [];
}

/**
 * Returns Keycloak realm roles decoded from the access token.
 * - Reads the token from localStorage synchronously on mount (no flash).
 * - Refreshes the token from better-auth on session change and updates localStorage.
 */
export function useRealmRoles(): string[] {
  const { data: session } = useSession();
  const [roles, setRoles] = useState<string[]>(() => {
    const cached = readToken();
    return cached ? getRolesFromToken(cached) : [];
  });

  useEffect(() => {
    if (!session) {
      clearToken();
      setRoles([]);
      return;
    }

    getAccessToken({ providerId: AUTH.PROVIDER })
      .then((result) => {
        const token = (result as { data?: { accessToken?: string } }).data
          ?.accessToken;
        if (!token) return;
        writeToken(token);
        setRoles(getRolesFromToken(token));
      })
      .catch(() => {
        // keep roles from cached token on transient errors
      });
  }, [session]);

  return roles;
}

/** Convenience hook — true if the current user has the "admin" realm role. */
export function useIsAdmin(): boolean {
  return useRealmRoles().includes("admin");
}
