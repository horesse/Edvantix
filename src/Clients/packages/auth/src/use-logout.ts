"use client";

import { useCallback } from "react";

import { useAuth } from "react-oidc-context";

/**
 * Returns a `logout` callback performing an RP-initiated logout via Keycloak.
 * The provider's `post_logout_redirect_uri` controls where the user lands after.
 */
export function useLogout() {
  const auth = useAuth();

  const logout = useCallback(async () => {
    try {
      await auth.signoutRedirect();
    } catch (error) {
      console.error("[auth] Logout failed:", error);
    }
  }, [auth]);

  return { logout };
}
