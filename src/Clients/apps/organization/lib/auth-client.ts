"use client";

import { genericOAuthClient } from "better-auth/client/plugins";
import { createAuthClient } from "better-auth/react";

export const authClient = createAuthClient({
  baseURL:
    globalThis.window === undefined
      ? "http://localhost:3000"
      : globalThis.location.origin,
  plugins: [genericOAuthClient()],
});

export const { signIn, signOut, useSession, getAccessToken } = authClient;

/**
 * Forces an unconditional token refresh via Keycloak, bypassing better-auth's
 * cache check. Unlike `getAccessToken`, which returns a cached token if it has
 * not yet expired, this always exchanges the stored refresh token for a brand-new
 * access token. Use this immediately after server-side user attribute changes
 * (e.g. `profileId` written after profile registration) so the new claims are
 * available in the next access token without waiting for natural expiry.
 */
export async function forceTokenRefresh(
  providerId: string,
): Promise<string | null> {
  const result = await authClient.$fetch<{ accessToken: string }>(
    "/refresh-token",
    {
      method: "POST",
      body: { providerId },
    },
  );

  return result.data?.accessToken ?? null;
}
