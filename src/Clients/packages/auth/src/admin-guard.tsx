"use client";

import type React from "react";

import { useAuth } from "react-oidc-context";

export interface AdminGuardProps {
  children: React.ReactNode;
  /** UI shown when the authenticated user lacks the `admin` realm role. */
  fallback: React.ReactNode;
}

/** Shape of Keycloak's `realm_access` claim, as found on the token profile. */
interface RealmAccess {
  roles?: string[];
}

/**
 * Guards routes that require the `admin` realm role.
 *
 * Reads roles from `auth.user.profile.realm_access` — no manual JWT decoding.
 *
 * @remarks Requires the Keycloak client to map realm roles into the ID token
 * (the "realm roles" mapper with "Add to ID token" enabled); otherwise
 * `realm_access` will be absent from the profile.
 */
export function AdminGuard({ children, fallback }: Readonly<AdminGuardProps>) {
  const auth = useAuth();
  const realmAccess = auth.user?.profile.realm_access as
    | RealmAccess
    | undefined;
  const isAdmin = realmAccess?.roles?.includes("admin") ?? false;

  if (!isAdmin) {
    return <>{fallback}</>;
  }

  return <>{children}</>;
}
