// Authentication
export const AUTH = {
  PROVIDER: "keycloak",
  CALLBACK_URL: "/",
} as const;

export const API = {
  DEFAULT_RETRY: 3,
  DEFAULT_TIMEOUT: 30000,
} as const;

export const PAGE_SIZES = [10, 20, 50] as const;
