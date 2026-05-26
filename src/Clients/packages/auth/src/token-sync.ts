import type { AuthContextProps } from "react-oidc-context";

import { registerTokenRefresher } from "@workspace/api-client/client";

/** localStorage key read by the api-client request interceptor. */
const ACCESS_TOKEN_KEY = "access_token";

/** Writes (or clears) the access token in localStorage. No-op on the server. */
function persistAccessToken(token: string | undefined): void {
  if (typeof window === "undefined") {
    return;
  }

  if (token) {
    window.localStorage.setItem(ACCESS_TOKEN_KEY, token);
  } else {
    window.localStorage.removeItem(ACCESS_TOKEN_KEY);
  }
}

/**
 * Mirrors the OIDC access token into `localStorage` under `access_token` so the
 * existing api-client request interceptor keeps working untouched. Re-runs on
 * every (silent) renewal via the `userLoaded` event.
 *
 * @returns an unsubscribe function to call on cleanup.
 */
export function syncAccessTokenToStorage(auth: AuthContextProps): () => void {
  persistAccessToken(auth.user?.access_token);

  return auth.events.addUserLoaded((user) => {
    persistAccessToken(user.access_token);
  });
}

/**
 * Registers the api-client 401 refresher to obtain a fresh token via OIDC silent
 * sign-in. Proactive refresh is owned by `automaticSilentRenew`; this only backs
 * the reactive 401-retry path in the interceptor.
 */
export function registerOidcTokenRefresher(auth: AuthContextProps): void {
  registerTokenRefresher(async () => {
    const user = await auth.signinSilent();

    return user?.access_token ?? null;
  });
}
