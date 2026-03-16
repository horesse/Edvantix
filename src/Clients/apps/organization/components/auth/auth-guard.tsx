"use client";

import { useEffect, useState } from "react";

import {
  registerTokenRefresher,
  unregisterTokenRefresher,
} from "@workspace/api-client/client";
import { isTokenExpiringSoon } from "@workspace/api-client/token-utils";

import { useUserContext } from "@/hooks/use-user-context";
import { getAccessToken, signIn, signOut } from "@/lib/auth-client";
import { AUTH } from "@/lib/constants";

import { LoadingScreen } from "./loading-screen";

/** Interval (ms) for proactive token expiry checks. */
const TOKEN_CHECK_INTERVAL_MS = 60_000;

/** Refresh the token this many ms before actual expiry. */
const TOKEN_REFRESH_BUFFER_MS = 5 * 60_000;

/**
 * Module-level flag preventing concurrent refresh calls.
 * Exists outside the component because there is only one auth session per tab.
 */
let isRefreshing = false;

/**
 * Exchanges the current session for a fresh Keycloak access token via
 * better-auth's token endpoint, then stores the result in localStorage.
 *
 * Defined at module scope so the reference is stable — no stale closures.
 */
async function fetchFreshToken(): Promise<string | null> {
  if (isRefreshing) return null;

  isRefreshing = true;

  try {
    const result = await getAccessToken({ providerId: AUTH.PROVIDER });
    const token = result.data?.accessToken ?? null;

    if (token) {
      globalThis.localStorage.setItem("access_token", token);
    }

    return token;
  } catch (error) {
    console.error("[auth-guard] Token refresh error:", error);
    return null;
  } finally {
    isRefreshing = false;
  }
}

/**
 * Guards all routes inside (main) layout.
 * - Unauthenticated users are redirected to the Keycloak login page.
 * - Proactively refreshes the access token before it expires.
 * - Registers a refresher for the Axios 401 interceptor.
 */
export function AuthGuard({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const { isAuthenticated, isLoading } = useUserContext();
  /**
   * Tracks whether the initial token fetch has completed.
   * Shows "session" stage briefly after auth is confirmed but before
   * the access token is stored and children can safely render.
   */
  const [tokenReady, setTokenReady] = useState(false);

  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      signIn.social({
        provider: AUTH.PROVIDER,
        callbackURL: AUTH.CALLBACK_URL,
      });
    }
  }, [isAuthenticated, isLoading]);

  useEffect(() => {
    if (!isAuthenticated) return;

    registerTokenRefresher(fetchFreshToken);

    // Fetch the initial token, then mark session as ready.
    fetchFreshToken().then(() => setTokenReady(true));

    const interval = setInterval(async () => {
      const currentToken = globalThis.localStorage.getItem("access_token");

      if (!isTokenExpiringSoon(currentToken, TOKEN_REFRESH_BUFFER_MS)) return;

      const newToken = await fetchFreshToken();

      if (!newToken) {
        console.warn("[auth-guard] Token refresh failed — signing out.");
        await signOut();
      }
    }, TOKEN_CHECK_INTERVAL_MS);

    return () => {
      clearInterval(interval);
      unregisterTokenRefresher();
    };
  }, [isAuthenticated]);

  // Checking session with Keycloak / better-auth.
  if (isLoading || !isAuthenticated) {
    return <LoadingScreen stage="auth" />;
  }

  // Session confirmed; fetching the initial access token.
  if (!tokenReady) {
    return <LoadingScreen stage="session" />;
  }

  return <>{children}</>;
}
