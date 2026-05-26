import { WebStorageStateStore } from "oidc-client-ts";
import type { AuthProviderProps } from "react-oidc-context";

/** Inputs for {@link buildOidcConfig}, supplied per-app at the provider boundary. */
export interface OidcConfigParams {
  /** OIDC issuer URL (Keycloak realm). */
  authority: string;
  /** Public client id registered in Keycloak. */
  clientId: string;
  /** Requested scopes; joined into the space-delimited `scope` value. */
  scopes: string[];
  /** Origin of the app, used to build redirect URIs. */
  appUrl: string;
}

/**
 * Strips the `?code=…&state=…` query left behind by the authorization-code
 * redirect so the SPA router starts from a clean URL.
 */
function clearSigninQueryParams(): void {
  window.history.replaceState({}, document.title, window.location.pathname);
}

/**
 * Builds the settings object passed to `react-oidc-context`'s `AuthProvider`.
 *
 * Uses the authorization-code flow with silent renewal handled automatically
 * via the refresh token (`automaticSilentRenew`). Tokens are persisted in
 * `localStorage` so they survive full page reloads.
 *
 * @remarks Must be called in the browser — it references `window.localStorage`.
 */
export function buildOidcConfig({
  authority,
  clientId,
  scopes,
  appUrl,
}: OidcConfigParams): AuthProviderProps {
  return {
    authority,
    client_id: clientId,
    redirect_uri: `${appUrl}/`,
    post_logout_redirect_uri: appUrl,
    scope: scopes.join(" "),
    response_type: "code",
    automaticSilentRenew: true,
    userStore: new WebStorageStateStore({ store: window.localStorage }),
    onSigninCallback: clearSigninQueryParams,
  };
}
