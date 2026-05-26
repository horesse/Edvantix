"use client";

import type React from "react";
import { useEffect, useState } from "react";

import { AuthProvider } from "react-oidc-context";

import { buildOidcConfig } from "./config";

export interface EdvantixAuthProviderProps {
  /** OIDC issuer URL (Keycloak realm). */
  authority: string;
  /** Public client id registered in Keycloak. */
  clientId: string;
  /** Requested scopes. */
  scopes: string[];
  /** Origin of the app, used to build redirect URIs. */
  appUrl: string;
  children: React.ReactNode;
}

/**
 * App-level OIDC provider wrapping `react-oidc-context`'s `AuthProvider`.
 *
 * The underlying `UserManager` touches `window.localStorage` when constructed,
 * so the provider defers building its config until after the first client
 * mount to stay safe during server-side rendering.
 */
export function EdvantixAuthProvider({
  authority,
  clientId,
  scopes,
  appUrl,
  children,
}: Readonly<EdvantixAuthProviderProps>) {
  const [mounted, setMounted] = useState(false);

  useEffect(() => {
    setMounted(true);
  }, []);

  if (!mounted) {
    return null;
  }

  const config = buildOidcConfig({ authority, clientId, scopes, appUrl });

  return <AuthProvider {...config}>{children}</AuthProvider>;
}
