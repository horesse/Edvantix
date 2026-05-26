"use client";

import { useAuth } from "react-oidc-context";

/**
 * Thin wrapper over `useAuth()` exposing the authenticated user's profile claims
 * alongside the session status flags.
 */
export function useUserContext() {
  const auth = useAuth();

  return {
    user: auth.user?.profile ?? null,
    isLoading: auth.isLoading,
    isAuthenticated: auth.isAuthenticated,
  };
}
