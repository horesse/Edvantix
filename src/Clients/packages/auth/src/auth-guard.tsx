"use client";

import type React from "react";
import { useEffect } from "react";

import { useAuth } from "react-oidc-context";

import { unregisterTokenRefresher } from "@workspace/api-client/client";

import {
  registerOidcTokenRefresher,
  syncAccessTokenToStorage,
} from "./token-sync";

export interface AuthGuardProps {
  children: React.ReactNode;
  /**
   * App-provided loading UI shown while the session is resolving or a redirect
   * to Keycloak is in flight. Lives in the app because it depends on app UI.
   */
  loadingScreen: React.ReactNode;
}

/**
 * Guards authenticated routes.
 * - Redirects unauthenticated users to the Keycloak login page.
 * - Mirrors the access token into localStorage and registers the api-client
 *   401 refresher for the lifetime of the session.
 *
 * Proactive token renewal is handled by `automaticSilentRenew` in the provider
 * config, so no polling timer is needed here.
 */
export function AuthGuard({
  children,
  loadingScreen,
}: Readonly<AuthGuardProps>) {
  const auth = useAuth();

  useEffect(() => {
    if (
      !auth.isLoading &&
      !auth.isAuthenticated &&
      !auth.activeNavigator &&
      !auth.error
    ) {
      void auth.signinRedirect();
    }
  }, [auth]);

  useEffect(() => {
    if (!auth.isAuthenticated) {
      return;
    }

    const unsubscribe = syncAccessTokenToStorage(auth);
    registerOidcTokenRefresher(auth);

    return () => {
      unsubscribe();
      unregisterTokenRefresher();
    };
  }, [auth]);

  if (auth.isLoading || !auth.isAuthenticated) {
    return <>{loadingScreen}</>;
  }

  return <>{children}</>;
}
