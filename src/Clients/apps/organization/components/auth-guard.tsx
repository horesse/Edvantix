"use client";

import { useEffect } from "react";

import {
  registerTokenRefresher,
  unregisterTokenRefresher,
} from "@workspace/api-client/client";
import { isTokenExpiringSoon } from "@workspace/api-client/token-utils";
import { Card, CardContent } from "@workspace/ui/components/card";
import { Spinner } from "@workspace/ui/components/spinner";

import { useUserContext } from "@/hooks/use-user-context";
import { getAccessToken, signIn, signOut } from "@/lib/auth-client";
import { AUTH } from "@/lib/constants";

/** How often (ms) to proactively check if the token needs refreshing. */
const TOKEN_CHECK_INTERVAL_MS = 60_000;

/** Refresh the token this many ms before it actually expires. */
const TOKEN_REFRESH_BUFFER_MS = 5 * 60_000;

/**
 * Module-level flag preventing concurrent refresh calls.
 * Lives outside the component because there is only one auth session per tab.
 */
let isRefreshing = false;

/**
 * Exchanges the current session's refresh token for a new Keycloak access token
 * via better-auth's server-side token endpoint, then persists the result.
 *
 * Defined at module level so the reference is stable — no stale closures in
 * effects and no need for useCallback or eslint-disable suppression.
 */
async function fetchFreshToken(): Promise<string | null> {
  if (isRefreshing) return null;

  isRefreshing = true;

  try {
    const result = await getAccessToken({ providerId: AUTH.PROVIDER });
    const token = result.data?.accessToken ?? null;

    if (token) {
      window.localStorage.setItem("access_token", token);
    }

    return token;
  } catch (error) {
    console.error("[auth-guard] Token refresh error:", error);
    return null;
  } finally {
    isRefreshing = false;
  }
}

export function LoadingScreen({
  title,
  description,
}: {
  title: string;
  description: string;
}) {
  return (
    <div className="from-background to-muted/20 flex h-screen items-center justify-center bg-linear-to-br">
      <Card
        className="border-muted/50 w-full max-w-md shadow-sm"
        role="status"
        aria-live="polite"
        aria-busy="true"
      >
        <CardContent className="space-y-6 pt-6 pb-6">
          <div className="flex justify-center" aria-hidden="true">
            <Spinner className="size-8" />
          </div>
          <div className="space-y-2 text-center">
            <h1 className="text-foreground text-xl font-semibold">{title}</h1>
            <p className="text-muted-foreground text-sm">{description}</p>
          </div>
          <span className="sr-only">{description}</span>
        </CardContent>
      </Card>
    </div>
  );
}

export function AuthGuard({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useUserContext();

  // Redirect unauthenticated users to Keycloak login.
  useEffect(() => {
    if (!isLoading && !isAuthenticated) {
      signIn.social({ provider: AUTH.PROVIDER, callbackURL: AUTH.CALLBACK_URL });
    }
  }, [isAuthenticated, isLoading]);

  // On authentication: fetch initial token, register the reactive refresher for
  // the Axios 401 interceptor, and start the proactive expiry check interval.
  useEffect(() => {
    if (!isAuthenticated) return;

    // Register so the Axios 401 interceptor can silently refresh and retry.
    registerTokenRefresher(fetchFreshToken);

    // Fetch immediately so the token is available before the first API call.
    fetchFreshToken();

    // Proactively renew the token before it expires to avoid 401 bursts.
    const interval = setInterval(async () => {
      const currentToken = window.localStorage.getItem("access_token");

      if (!isTokenExpiringSoon(currentToken, TOKEN_REFRESH_BUFFER_MS)) return;

      const newToken = await fetchFreshToken();

      if (!newToken) {
        // Refresh token itself has expired — force the user to re-authenticate.
        console.warn("[auth-guard] Token refresh failed — signing out.");
        await signOut();
      }
    }, TOKEN_CHECK_INTERVAL_MS);

    return () => {
      clearInterval(interval);
      unregisterTokenRefresher();
    };
  }, [isAuthenticated]);

  if (isLoading) {
    return (
      <LoadingScreen
        title="Загрузка"
        description="Пожалуйста, подождите, идёт проверка учётных данных..."
      />
    );
  }

  if (!isAuthenticated) {
    return (
      <LoadingScreen
        title="Требуется аутентификация"
        description="Перенаправление на страницу входа..."
      />
    );
  }

  return <>{children}</>;
}
