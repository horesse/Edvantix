"use client";

import type React from "react";
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

const TOKEN_CHECK_INTERVAL_MS = 60_000;
const TOKEN_REFRESH_BUFFER_MS = 5 * 60_000;

let isRefreshing = false;

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

export function AuthGuard({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  const { isAuthenticated, isLoading } = useUserContext();
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

  if (isLoading || !isAuthenticated) {
    return <LoadingScreen stage="auth" />;
  }

  if (!tokenReady) {
    return <LoadingScreen stage="session" />;
  }

  return <>{children}</>;
}
