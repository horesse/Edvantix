"use client";

import { useCallback } from "react";

import { useAuth } from "react-oidc-context";

const ACCESS_TOKEN_KEY = "access_token";

/**
 * Returns a callback that performs a silent OIDC token refresh and persists
 * the new access token to localStorage under the `access_token` key.
 *
 * Use after operations that change Keycloak claims (e.g. profile registration)
 * to ensure the next API request carries a token with the updated claims.
 */
export function useTokenRefresh(): () => Promise<string | null> {
  const auth = useAuth();

  return useCallback(async () => {
    const user = await auth.signinSilent();

    if (user?.access_token) {
      window.localStorage.setItem(ACCESS_TOKEN_KEY, user.access_token);
    }

    return user?.access_token ?? null;
  }, [auth]);
}
